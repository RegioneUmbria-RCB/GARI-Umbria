
Imports System.Globalization
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MeteoNT
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiStazione(Id_Stazione As Integer, needSensors As Boolean, objParametriServer As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "MeteoNT.LeggiStazione"

        Try

            Dim sql As New StringBuilder

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ID INTEGER = " & Id_Stazione.ToString())
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	z.Id ")
            sql.AppendLine("	, Stazione = z.Nome ")
            sql.AppendLine("	, Lat = COALESCE(GeoEntity.EnvelopeCenter().Lat, Lat_Dec) ")
            sql.AppendLine("	, Lng = COALESCE(GeoEntity.EnvelopeCenter().Long, Lng_Dec) ")
            sql.AppendLine("	, FlagReale ")
            sql.AppendLine("	, RifFornitore ")
            sql.AppendLine("	, Fornitore = f.Descrizione ")
            sql.AppendLine("FROM MeteoNT_Stazioni z ")
            sql.AppendLine("INNER JOIN MeteoNT_Fornitori f ON f.Id = z.Id_Fornitore ")
            sql.AppendLine("WHERE z.Id = @ID ")

            If needSensors Then
                sql.AppendLine()
                sql.AppendLine("SELECT ")
                sql.AppendLine("	sxs1.Id_Sensore ")
                sql.AppendLine("	, Sensore = COALESCE(sxs1.Etichetta, s1.Etichetta, '') ")
                sql.AppendLine("	, Tipo = t1.Tipo ")
                sql.AppendLine("	, t1.UM ")
                sql.AppendLine("	, Id_StazioneOrigine = z2.Id ")
                sql.AppendLine("	, StazioneOrigine = z2.Nome ")
                sql.AppendLine("	, dati.Last_Update ")
                sql.AppendLine("FROM MeteoNT_StazioniXSensori sxs1 ")
                sql.AppendLine("INNER JOIN MeteoNT_Sensori s1 on s1.Id = sxs1.Id_Sensore ")
                sql.AppendLine("INNER JOIN MeteoNT_TipiSensore t1 ON t1.Id = s1.Id_TipoSensore ")
                sql.AppendLine("INNER JOIN MeteoNT_Stazioni z1 ON z1.Id = sxs1.Id_Stazione ")
                sql.AppendLine("LEFT JOIN MeteoNT_StazioniXSensori sxs2 ON sxs2.Id_Sensore = sxs1.Id_Sensore AND z1.FlagReale = 0 ")
                sql.AppendLine("LEFT JOIN MeteoNT_Stazioni z2 ON z2.Id = sxs2.Id_Stazione AND z2.FlagReale = 1 ")
                sql.AppendLine("LEFT JOIN ( ")
                sql.AppendLine("	SELECT Id_Sensore, Last_Update = MAX(DataOra) FROM MeteoNT_DatiOrari GROUP BY Id_Sensore ")
                sql.AppendLine(") dati ON dati.Id_Sensore = sxs1.Id_Sensore ")
                sql.AppendLine("WHERE z1.Id = @ID AND (z1.FlagReale = 1 OR z2.Id IS NOT NULL) ")
                sql.AppendLine("ORDER BY sxs1.Ordine ")
            End If

            Dim DS As New DataSet

            EseguiQuery_Lettura(objParametriServer, sql.ToString(), NomeRoutine, DS, "DatiMeteo_NT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)

        End Try

        Return Nothing
    End Function


    Public Function LeggiSorgentiMeteo(PIVA_Superuser As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiSorgentiMeteo"

        Try
            '0 (enum_Meteo_Tiposorgente.Gias_RER)               "Gias (Stazioni Regione E.R.)"
            '3 (enum_Meteo_Tiposorgente.Gias_RER_Quadranti)     "Gias (Quadranti Regione E.R.)"
            '4 (enum_Meteo_Tiposorgente.Pubbliche)              "Stazioni pubbliche"
            '1 (enum_Meteo_Tiposorgente.RetiPartner)            "Tutte le stazioni in visibilità"
            '2 (enum_Meteo_Tiposorgente.Aziendali)              "Solo le stazioni aziendali"

            'TODO: Tabella che permetta la gestione delle sorgenti meteo visibili da PIVA_Superuser diverse...
            'Dim sql As String = "SELECT Id, Descrizione FROM MeteoNT_Sorgenti ORDER By Ordine"

            Dim sql As New StringBuilder

            sql.AppendLine($"DECLARE @PIVA_Superuser VARCHAR(50) = '{PIVA_Superuser}';")
            sql.AppendLine()
            sql.AppendLine("WITH SORGENTI_CTE AS (")
            sql.AppendLine("	SELECT Id_Sorgente = s.Id, Descrizione = COALESCE(v.Alias, s.Descrizione), Ordine, Scope = IIF(v.Id_Sorgente IS NULL, 1, 0)")
            sql.AppendLine("	FROM MeteoNT_Sorgenti s")
            sql.AppendLine("	LEFT JOIN MeteoNT_VisibilitaSorgenti v ON v.Id_Sorgente = s.Id AND v.PIVA_Superuser = @PIVA_Superuser")
            sql.AppendLine(")")
            sql.AppendLine()
            sql.AppendLine("SELECT Id = Id_Sorgente, Descrizione ")
            sql.AppendLine("FROM SORGENTI_CTE ")
            sql.AppendLine("WHERE Scope = (SELECT MIN(Scope) FROM SORGENTI_CTE) ")
            sql.AppendLine("ORDER BY Ordine")


            Return EseguiQuery_Lettura(ObjParametri_Server, sql.ToString(), NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)

        End Try

        Return Nothing
    End Function


    Public Function LeggiStazioniConDistanza(ByVal TipoSorgente As enum_Meteo_Tiposorgente, ByVal lat As Double, ByVal lng As Double, ByVal piva_superuser As String, ByVal piva As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiStazioniConDistanza"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim sql_T1 As New StringBuilder
        Dim sql_T2 As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql_T1.Clear()
            sql_T2.Clear()

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER

                    sql_T1.AppendLine("     SELECT S.ID_Stazione AS ID, Stazione_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ")
                    sql_T1.AppendLine("     FROM TB_Stazioni S ")
                    sql_T1.AppendLine("		INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
                    sql_T1.AppendLine("		INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")

                    sql_T2.AppendLine("		SELECT S.ID_Stazione AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM TB_Stazioni S ")
                    sql_T2.AppendLine("		LEFT JOIN Dati_Meteo_SHH D ON D.ID_Stazione = S.ID_Stazione ")
                    sql_T2.AppendLine("		GROUP BY S.ID_Stazione")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sql_T1.AppendLine("     SELECT Q.ID_Quadrante AS ID, Quadrante_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ")
                    sql_T1.AppendLine("     FROM TB_Quadranti Q ")

                    sql_T2.AppendLine("		SELECT Q.ID_Quadrante AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM TB_Quadranti Q ")
                    sql_T2.AppendLine("		LEFT JOIN Dati_Meteo_QHH D ON D.ID_Quadrante = Q.ID_Quadrante ")
                    sql_T2.AppendLine("		GROUP BY Q.ID_Quadrante ")

                Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali

                    sql_T1.AppendLine("		SELECT ")
                    sql_T1.AppendLine("         ID = S.Id ")
                    sql_T1.AppendLine("         , Descrizione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sql_T1.AppendLine("         , Fornitore = CASE WHEN vis.Anonima = 0 THEN COALESCE(vis.DescrizioneAggiuntiva, F.Descrizione, '') ELSE '' END ")
                    sql_T1.AppendLine("         , RifFornitore = CASE WHEN vis.Anonima = 0 THEN S.RifFornitore ELSE '' END ")
                    sql_T1.AppendLine("         , Lat = CAST(S.Lat_Dec AS VARCHAR(50)) ")
                    sql_T1.AppendLine("         , Lng = CAST(S.Lng_Dec AS VARCHAR(50)) ")
                    sql_T1.AppendLine("     FROM MeteoNT_Stazioni S ")
                    sql_T1.AppendLine("     INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sql_T1.AppendLine("     INNER JOIN ( ")
                    sql_T1.AppendLine("         SELECT Id_Stazione, DescrizioneAggiuntiva, Anonima FROM ")
                    sql_T1.AppendLine("         ( ")
                    sql_T1.AppendLine("             SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Stazione ORDER BY Id_Stazione ASC, Proprietario DESC) AS Cnt ")
                    sql_T1.AppendLine("             FROM ( ")
                    sql_T1.AppendLine("		                SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("		                FROM MeteoNT_VisibilitaStazioni ")
                    sql_T1.AppendLine("                     WHERE PIVA_Superuser = '" & piva_superuser & "' AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("		                UNION ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("             		SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("		                FROM MeteoNT_VisibilitaStazioni ")
                    sql_T1.AppendLine("		                WHERE PIVA_Superuser = '" & piva_superuser & "' AND PIVA = '*' ")
                    sql_T1.AppendLine("             ) TBL1 ")

                    If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali Then
                        sql_T1.AppendLine("             WHERE Proprietario = 1 --se solo stazioni aziendali ")
                    End If

                    sql_T1.AppendLine("         ) TBL2 ")
                    sql_T1.AppendLine("         WHERE Cnt = 1 ")
                    sql_T1.AppendLine("     ) vis on vis.Id_Stazione = S.Id")

                    sql_T2.AppendLine("		SELECT Id_Stazione AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM MeteoNT_StazioniXSensori sxs ")
                    sql_T2.AppendLine("		LEFT JOIN (SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		    FROM MeteoNT_DatiOrari ")
                    sql_T2.AppendLine("		    GROUP BY Id_Sensore) s ON s.Id_Sensore = sxs.Id_Sensore ")
                    sql_T2.AppendLine("		GROUP BY Id_Stazione ")

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

    Public Function LeggiStazioniAutorizzate(piva_superuser As String, piva As String, tipo_sorgente As Integer, coord As LatLng, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiStazioniAutorizzate"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder
            Dim sql_T1 As New StringBuilder
            Dim sql_T2 As New StringBuilder

            sql.Clear()
            sql_T1.Clear()
            sql_T2.Clear()

            Select Case tipo_sorgente

                Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali

                    sql_T1.AppendLine("     SELECT ")
                    sql_T1.AppendLine("         Id_Stazione = S.Id ")
                    sql_T1.AppendLine("         , Nome_Stazione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sql_T1.AppendLine("			, Lat = Lat_Dec ")
                    sql_T1.AppendLine("			, Lng = Lng_Dec ")
                    sql_T1.AppendLine("			, Fornitore = F.Descrizione ")
                    sql_T1.AppendLine("			, RifFornitore ")
                    sql_T1.AppendLine("			, FlagReale ")
                    sql_T1.AppendLine(" 	FROM MeteoNT_Stazioni S WITH (NOLOCK) ")
                    sql_T1.AppendLine("     INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sql_T1.AppendLine("     INNER JOIN ( ")
                    sql_T1.AppendLine("		    SELECT Id_Stazione, DescrizioneAggiuntiva, Anonima FROM ")
                    sql_T1.AppendLine("		    ( ")
                    sql_T1.AppendLine("			    SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Stazione ORDER BY Id_Stazione ASC, Proprietario DESC) AS Cnt ")
                    sql_T1.AppendLine("	            FROM ( ")
                    sql_T1.AppendLine("				    SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("				    FROM MeteoNT_VisibilitaStazioni ")
                    sql_T1.AppendLine("				    WHERE PIVA_Superuser = @PIVA_SUPERUSER AND PIVA = @PIVA ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				    UNION ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				    SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("				    FROM MeteoNT_VisibilitaStazioni ")
                    sql_T1.AppendLine("				    WHERE PIVA_Superuser = @PIVA_SUPERUSER AND PIVA = '*' ")
                    sql_T1.AppendLine("		        ) TBL1 ")

                    If tipo_sorgente = enum_Meteo_Tiposorgente.Aziendali Then
                        sql_T1.AppendLine("             WHERE Proprietario = 1 --se solo stazioni aziendali ")
                    End If

                    sql_T1.AppendLine("         ) TBL2 ")
                    sql_T1.AppendLine("         WHERE Cnt = 1 ")
                    sql_T1.AppendLine("     ) vis on vis.Id_Stazione = S.Id ")

                    sql_T2.AppendLine("     SELECT Id_Stazione AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("     FROM MeteoNT_StazioniXSensori sxs WITH (NOLOCK)")
                    sql_T2.AppendLine("     LEFT JOIN ( ")
                    sql_T2.AppendLine("         SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento FROM MeteoNT_DatiOrari WITH (NOLOCK) GROUP BY Id_Sensore ")
                    sql_T2.AppendLine("     ) s ON s.Id_Sensore = sxs.Id_Sensore ")
                    sql_T2.AppendLine("		GROUP BY Id_Stazione ")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sql_T1.AppendLine("     SELECT ")
                    sql_T1.AppendLine("         Id_Stazione = Q.ID_Quadrante ")
                    sql_T1.AppendLine("         , Nome_Stazione = Quadrante_Des ")
                    sql_T1.AppendLine("         , Lat = CAST(replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS FLOAT) ")
                    sql_T1.AppendLine("         , Lng = CAST(replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT) ")
                    sql_T1.AppendLine("         , Fornitore = '' ")
                    sql_T1.AppendLine("         , RifFornitore = '' ")
                    sql_T1.AppendLine("         , FlagReale = 1 ")
                    sql_T1.AppendLine("     FROM TB_Quadranti Q ")

                    sql_T2.AppendLine("		SELECT ID_Quadrante AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM Dati_Meteo_QHH ")
                    sql_T2.AppendLine("		GROUP BY ID_Quadrante ")

                Case enum_Meteo_Tiposorgente.Gias_RER

                    sql_T1.AppendLine("     SELECT ")
                    sql_T1.AppendLine("         Id_Stazione = S.ID_Stazione ")
                    sql_T1.AppendLine("         , Nome_Stazione = Stazione_Des ")
                    sql_T1.AppendLine("         , Lat = CAST(replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS FLOAT) ")
                    sql_T1.AppendLine("         , Lng = CAST(replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT) ")
                    sql_T1.AppendLine("         , Fornitore = '' ")
                    sql_T1.AppendLine("         , RifFornitore = '' ")
                    sql_T1.AppendLine("         , FlagReale = 1 ")
                    sql_T1.AppendLine("     FROM TB_Stazioni S ")
                    sql_T1.AppendLine("     INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
                    sql_T1.AppendLine("     INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")

                    sql_T2.AppendLine("		SELECT ID_Stazione AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM Dati_Meteo_SHH ")
                    sql_T2.AppendLine("		GROUP BY ID_Stazione")

                Case enum_Meteo_Tiposorgente.Pubbliche

                    sql_T1.AppendLine("	SELECT")
                    sql_T1.AppendLine("		Id_Stazione = S.Id")
                    sql_T1.AppendLine("		, Nome_Stazione = IIF(Z.Alias = 1, A.Alias, F.Descrizione + ' (' + S.Nome + ')')")
                    sql_T1.AppendLine("		, Lat = S.GeoEntity.EnvelopeCenter().Lat")
                    sql_T1.AppendLine("		, Lng = S.GeoEntity.EnvelopeCenter().Long")
                    sql_T1.AppendLine("		, Fornitore = F.Descrizione")
                    sql_T1.AppendLine("		, RifFornitore")
                    sql_T1.AppendLine("		, FlagReale")
                    sql_T1.AppendLine("	FROM (")
                    sql_T1.AppendLine("		SELECT Id_Stazione, Alias FROM (")
                    sql_T1.AppendLine("			SELECT Id_Stazione, Alias, rownum = ROW_NUMBER() OVER (PARTITION BY Id_Stazione ORDER BY Alias DESC)")
                    sql_T1.AppendLine("			FROM (")
                    sql_T1.AppendLine("				SELECT Id_Stazione, Alias = 0")
                    sql_T1.AppendLine("				FROM (")
                    sql_T1.AppendLine("					SELECT TOP 1 Id_Stazione = Id, Dist = GeoEntity.STDistance(@SRCPOINT)")
                    sql_T1.AppendLine("					FROM MeteoNT_Stazioni WITH (NOLOCK)")
                    sql_T1.AppendLine("					WHERE Categoria = 1 AND @SRCPOINT IS NOT NULL")
                    sql_T1.AppendLine("					ORDER BY Dist")
                    sql_T1.AppendLine("				) T")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				UNION ALL")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				SELECT Id_Stazione = S.Id, Alias = 1")
                    sql_T1.AppendLine("				FROM MeteoNT_Stazioni S WITH (NOLOCK)")
                    sql_T1.AppendLine("				INNER JOIN MeteoNT_VisibilitaStazioniAlias A WITH (NOLOCK) ON A.Id_Stazione = S.Id AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA")
                    sql_T1.AppendLine("			) T")
                    sql_T1.AppendLine("		) T WHERE rownum = 1")
                    sql_T1.AppendLine("	) Z")
                    sql_T1.AppendLine("	INNER JOIN MeteoNT_Stazioni S WITH (NOLOCK) ON S.Id = Z.Id_Stazione")
                    sql_T1.AppendLine("	INNER JOIN MeteoNT_Fornitori F WITH (NOLOCK) ON F.Id = S.Id_Fornitore")
                    sql_T1.AppendLine("	LEFT JOIN MeteoNT_VisibilitaStazioniAlias A WITH (NOLOCK) ON A.Id_Stazione = Z.Id_Stazione AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA")

                    sql_T2.AppendLine("	SELECT sxs.Id_Stazione AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento")
                    sql_T2.AppendLine("	FROM MeteoNT_StazioniXSensori sxs WITH (NOLOCK)")
                    sql_T2.AppendLine("	INNER JOIN STAZ_CTE z on z.Id_Stazione = sxs.Id_Stazione")
                    sql_T2.AppendLine("    LEFT JOIN (")
                    sql_T2.AppendLine("		SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento FROM MeteoNT_DatiOrari WITH (NOLOCK) GROUP BY Id_Sensore")
                    sql_T2.AppendLine("	) s ON s.Id_Sensore = sxs.Id_Sensore")
                    sql_T2.AppendLine("	GROUP BY sxs.Id_Stazione")

            End Select

            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine("DECLARE @SRCPOINT AS GEOGRAPHY = NULL ")

            If coord IsNot Nothing Then

                sql.AppendLine($"SET @SRCPOINT = GEOGRAPHY::Point({coord.Lat.ToString(CultureInfo.InvariantCulture)}, {coord.Lng.ToString(CultureInfo.InvariantCulture)}, 4326) ")
            End If

            sql.AppendLine()
            sql.AppendLine(";WITH STAZ_CTE AS ( ")
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
            sql.AppendLine("	FROM STAZ_CTE S ")
            sql.AppendLine("	LEFT JOIN AGG_CTE A ON A.ID = S.Id_Stazione ")
            sql.AppendLine(") T ")
            sql.AppendLine("ORDER BY Ordine, Distanza, Nome_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return DT
    End Function


    Public Function LeggiAnagraficaStazioni(piva_superuser As String, piva As String, id_stazione As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiAnagraficaStazioni"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @PIVA_SUPERUSER VARCHAR(50) = '" & piva_superuser & "' ")
            sql.AppendLine("DECLARE @PIVA VARCHAR(50) = '" & Agro_SQL_SaveText(piva) & "' ")
            sql.AppendLine("DECLARE @NO_UPDATE DATETIME = 0 ")
            sql.AppendLine()
            sql.AppendLine(";WITH STAZIONI_AUTORIZZATE_CTE AS ( ")
            sql.AppendLine("	SELECT z.Id, a.Proprietario ")
            sql.AppendLine("	FROM MeteoNT_Stazioni z WITH (NOLOCK) ")
            sql.AppendLine("	INNER JOIN ( ")
            sql.AppendLine("		SELECT Id_Stazione, Proprietario FROM ( ")
            sql.AppendLine("			SELECT Id_Stazione, Proprietario, RowNum = ROW_NUMBER() OVER(PARTITION BY Id_Stazione ORDER BY Id_Stazione ASC, Proprietario DESC) ")
            sql.AppendLine("				FROM ( ")
            sql.AppendLine("					SELECT Id_Stazione, Proprietario FROM MeteoNT_VisibilitaStazioni WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = @PIVA And Anonima = 0 ")
            sql.AppendLine("					UNION ")
            sql.AppendLine("					SELECT Id_Stazione, Proprietario FROM MeteoNT_VisibilitaStazioni WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = '*' AND Anonima = 0 ")
            sql.AppendLine("				) T ")
            sql.AppendLine("		) T WHERE RowNum = 1 ")
            sql.AppendLine("	) a ON a.Id_Stazione = z.Id ")
            sql.AppendLine(") ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id_Stazione = z1.Id ")
            sql.AppendLine("	, Fornitore = f.Descrizione ")
            sql.AppendLine("	, Stazione = z1.Nome ")
            sql.AppendLine("	, z1.Lat_Dec ")
            sql.AppendLine("	, z1.Lng_Dec ")
            sql.AppendLine("	, z1.FlagReale ")
            sql.AppendLine("	, z1.RifFornitore ")
            sql.AppendLine("	, za.Proprietario ")
            sql.AppendLine("	, Id_Sensore = s.Id ")
            sql.AppendLine("	, Sensore = COALESCE(x1.Etichetta, s.Etichetta, '') ")
            sql.AppendLine("	, Tipo_Sensore = t.Tipo ")
            sql.AppendLine("	, t.UM ")
            sql.AppendLine("	, OutputConfig = COALESCE(x1.OutputConfig, t.DefaultOutputConfig) ")
            sql.AppendLine("	, Id_StazioneOrigine = IIF(z2.Id = z1.Id, 0, z2.Id) ")
            sql.AppendLine("	, StazioneOrigine = IIF(z2.Id = z1.Id, '', z2.Nome) ")
            sql.AppendLine("	, Last_Update = COALESCE(dati.Last_Update, @NO_UPDATE) ")
            sql.AppendLine("FROM STAZIONI_AUTORIZZATE_CTE za ")
            sql.AppendLine("INNER JOIN MeteoNT_Stazioni z1 WITH (NOLOCK) on z1.Id = za.Id ")
            sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori x1 WITH (NOLOCK) on x1.Id_Stazione = z1.Id ")
            sql.AppendLine("INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) on s.Id = x1.Id_Sensore ")
            sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori x2 on x2.Id_Sensore = s.Id ")
            sql.AppendLine("INNER JOIN MeteoNT_Stazioni z2 WITH (NOLOCK) on z2.Id = x2.Id_Stazione AND z2.FlagReale = 1 ")
            sql.AppendLine("INNER JOIN MeteoNT_TipiSensore t WITH (NOLOCK) ON t.Id = s.Id_TipoSensore ")
            sql.AppendLine("INNER JOIN MeteoNT_Fornitori f WITH (NOLOCK) ON f.Id = z1.Id_Fornitore ")
            sql.AppendLine("LEFT JOIN ( ")
            sql.AppendLine("	SELECT Id_Sensore, Last_Update = MAX(DataOra) FROM MeteoNT_DatiSorgente WITH (NOLOCK) GROUP BY Id_Sensore ")
            sql.AppendLine(") dati ON dati.Id_Sensore = s.Id ")

            'sql.AppendLine(";WITH STAZIONI_AUTORIZZATE_CTE AS ( ")
            'sql.AppendLine("	SELECT staz.Id, FlagReale, aut.Proprietario ")
            'sql.AppendLine("	FROM MeteoNT_Stazioni staz ")
            'sql.AppendLine("	INNER JOIN ( ")
            'sql.AppendLine("		SELECT Id_Stazione, Proprietario FROM ( ")
            'sql.AppendLine("			SELECT Id_Stazione, Proprietario, RowNum = ROW_NUMBER() OVER(PARTITION BY Id_Stazione ORDER BY Id_Stazione ASC, Proprietario DESC) ")
            'sql.AppendLine("				FROM ( ")
            'sql.AppendLine("					SELECT Id_Stazione, Proprietario FROM MeteoNT_VisibilitaStazioni WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = @PIVA And Anonima = 0 ")
            'sql.AppendLine("					UNION ")
            'sql.AppendLine("					SELECT Id_Stazione, Proprietario FROM MeteoNT_VisibilitaStazioni WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = '*' AND Anonima = 0 ")
            'sql.AppendLine("				) T ")
            'sql.AppendLine("		) T WHERE RowNum = 1 ")
            'sql.AppendLine("	) aut ON aut.Id_Stazione = staz.Id ")
            'sql.AppendLine("), ")
            'sql.AppendLine()
            'sql.AppendLine("STAZIONI_SENSORI_CTE AS ( ")
            'sql.AppendLine("	SELECT Id_Stazione = z.Id, z.FlagReale, z.Proprietario, Id_Sensore, sxs.Ordine, Id_StazioneOrigine = NULL ")
            'sql.AppendLine("	FROM STAZIONI_AUTORIZZATE_CTE z ")
            'sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Stazione = z.Id ")
            'sql.AppendLine("	WHERE z.FlagReale = 1 ")
            'sql.AppendLine()
            'sql.AppendLine("	UNION ALL ")
            'sql.AppendLine()
            'sql.AppendLine("	SELECT Id_Stazione, FlagReale, Proprietario, Id_Sensore, Ordine, Id_StazioneOrigine FROM ( ")
            'sql.AppendLine("		SELECT Id_Stazione = z.Id, z.FlagReale, z.Proprietario, sxs1.Id_Sensore, sxs1.Ordine,Id_StazioneOrigine = z1.Id, Idx = ROW_NUMBER() OVER (PARTITION BY sxs1.Id_Stazione, sxs1.Id_Sensore ORDER BY sxs2.Id_Stazione) ")
            'sql.AppendLine("		FROM STAZIONI_AUTORIZZATE_CTE z ")
            'sql.AppendLine("		INNER JOIN MeteoNT_StazioniXSensori sxs1 ON sxs1.Id_Stazione = z.Id ")
            'sql.AppendLine("		LEFT JOIN MeteoNT_StazioniXSensori sxs2 ON sxs2.Id_Sensore = sxs1.Id_Sensore ")
            'sql.AppendLine("		LEFT JOIN MeteoNT_Stazioni z1 ON z1.Id = sxs2.Id_Stazione AND z1.FlagReale = 1 ")
            'sql.AppendLine("		WHERE z.FlagReale = 0 ")
            'sql.AppendLine("	) T WHERE Idx = 1 ")
            'sql.AppendLine(") ")
            'sql.AppendLine()
            'sql.AppendLine("SELECT ")
            'sql.AppendLine("	cte.Id_Stazione ")
            'sql.AppendLine("	, Fornitore = f.Descrizione ")
            'sql.AppendLine("	, Stazione = z1.Nome ")
            'sql.AppendLine("	, z1.Lat_Dec ")
            'sql.AppendLine("	, z1.Lng_Dec ")
            'sql.AppendLine("	, cte.FlagReale ")
            'sql.AppendLine("	, z1.RifFornitore ")
            'sql.AppendLine("	, cte.Proprietario ")
            'sql.AppendLine("	, cte.Id_Sensore ")
            'sql.AppendLine("	, Sensore = COALESCE(sxs.Etichetta, s.Etichetta, '') ")
            'sql.AppendLine("	, Tipo_Sensore = t.Tipo ")
            'sql.AppendLine("	, t.UM ")
            'sql.AppendLine("	, OutputConfig = COALESCE(sxs.OutputConfig, t.DefaultOutputConfig) ")
            'sql.AppendLine("	, Id_StazioneOrigine = COALESCE(cte.Id_StazioneOrigine, 0) ")
            'sql.AppendLine("	, StazioneOrigine = COALESCE(z2.Nome, '') ")
            'sql.AppendLine("	, Last_Update = COALESCE(dati.Last_Update, @NO_UPDATE) ")
            'sql.AppendLine("FROM STAZIONI_SENSORI_CTE cte ")
            'sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Stazione = cte.Id_Stazione AND sxs.Id_Sensore = cte.Id_Sensore ")
            'sql.AppendLine("INNER JOIN MeteoNT_Stazioni z1 ON z1.Id = cte.Id_Stazione ")
            'sql.AppendLine("LEFT JOIN MeteoNT_Stazioni z2 ON z2.Id = cte.Id_StazioneOrigine ")
            'sql.AppendLine("INNER JOIN MeteoNT_Sensori s ON s.Id = cte.Id_Sensore ")
            'sql.AppendLine("INNER JOIN MeteoNT_TipiSensore t ON t.Id = s.Id_TipoSensore ")
            'sql.AppendLine("INNER JOIN MeteoNT_Fornitori f ON f.Id = z1.Id_Fornitore ")
            'sql.AppendLine("LEFT JOIN ( ")
            'sql.AppendLine("	SELECT Id_Sensore, Last_Update = MAX(DataOra) FROM MeteoNT_DatiSorgente GROUP BY Id_Sensore ")
            'sql.AppendLine(") dati ON dati.Id_Sensore = cte.Id_Sensore ")

            If id_stazione > 0 Then

                'sql.AppendLine("WHERE cte.Id_Stazione = " & id_stazione.ToString)
                sql.AppendLine("WHERE za.Id = " & id_stazione.ToString)
            End If

            'sql.AppendLine("ORDER BY cte.Id_Stazione, cte.Ordine ")
            sql.AppendLine("ORDER BY za.id, x1.ordine ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function


    Public Function LeggiIdStazioni(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.LeggiIdStazioni"
        Dim Riepilogo As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT DISTINCT s." & tblSensori.CampoIdStazione)
            sql.AppendLine("	FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id ")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function


    Public Function LeggiTipoSensoriPerStazione(ByVal id_stazione As Integer, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiTipoSensoriPerStazione"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql.Clear()

            sql.AppendLine("DECLARE @ID_STAZIONE INT = " & id_stazione & " ")
            sql.AppendLine("DECLARE @NO_UPDATE DATETIME = 0 ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id_Stazione ")
            sql.AppendLine("	, Id_Sensore ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, Tipo ")
            sql.AppendLine("	, UM ")
            sql.AppendLine("FROM ")
            sql.AppendLine("    MeteoNT_TipiSensore A ")
            sql.AppendLine("    INNER JOIN MeteoNT_Sensori B on (A.Id=B.Id_TipoSensore) ")
            sql.AppendLine("    INNER JOIN MeteoNT_StazioniXSensori C on (C.Id_Sensore=B.Id) ")

            If id_stazione > 0 Then

                sql.AppendLine("WHERE Id_Stazione = @ID_STAZIONE")
            End If

            sql.AppendLine("ORDER BY Id_Stazione,Id_Sensore ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT
    End Function


    Public Class AnagStazione
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


    Public Function AggiornaAnagraficaStazione(piva_superuser As String, piva As String, Anag As AnagStazione, ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.AggiornaAnagraficaStazione"

        Dim Id_Stazione As Integer = 0
        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}';")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}';")
            sql.AppendLine($"DECLARE @ID_STAZIONE AS INT = {Anag.Id};")
            sql.AppendLine($"DECLARE @STAZIONE AS VARCHAR(MAX) = '{Anag.Nome}';")
            sql.AppendLine($"DECLARE @LAT AS FLOAT = {If(Anag.Lat.HasValue, Convert.ToString(Anag.Lat.Value, CultureInfo.InvariantCulture), "NULL")}")
            sql.AppendLine($"DECLARE @LNG AS FLOAT = {If(Anag.Lng.HasValue, Convert.ToString(Anag.Lng.Value, CultureInfo.InvariantCulture), "NULL")}")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ALLOW_UPDATE AS INT = 0;")
            sql.AppendLine()
            sql.AppendLine("DECLARE @SXS AS TABLE (ID_STAZIONE INT, ID_SENSORE INT, ORDINE INT, ETICHETTA VARCHAR(MAX), OUTPUTCONFIG VARCHAR(MAX))")
            sql.AppendLine("INSERT INTO @SXS VALUES ")
            Dim sxs As New List(Of String)
            Dim ordine As Integer = 1
            For Each sens In Anag.Sensori
                sxs.Add($"(@ID_STAZIONE, {sens.Id}, {ordine}, '{sens.Etichetta}', {If(String.IsNullOrEmpty(sens.OutputConfig), "NULL", $"'{sens.OutputConfig}'")})")
                ordine += 1
            Next
            sql.AppendLine(String.Join("," & vbCrLf, sxs))
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY")
            sql.AppendLine()
            sql.AppendLine("	BEGIN TRANSACTION")
            sql.AppendLine()
            sql.AppendLine("	IF @ID_STAZIONE <= 0")
            sql.AppendLine("	BEGIN")
            sql.AppendLine()
            sql.AppendLine("		--Inserimento nuova stazione virtuale")
            sql.AppendLine()
            sql.AppendLine("		SET @ID_STAZIONE = (SELECT MAX(Id) FROM MeteoNT_Stazioni) + 1")
            sql.AppendLine()
            sql.AppendLine("		DECLARE @ID_FORNITORE AS INT;")
            sql.AppendLine("		DECLARE @RIFFORNITORE AS VARCHAR(MAX);")
            sql.AppendLine("		DECLARE @DESCRIZIONE AS VARCHAR(MAX);")
            sql.AppendLine($"		DECLARE @NOTE AS VARCHAR(MAX) = '{Anag.NoteVisibilita}';")
            sql.AppendLine()
            sql.AppendLine("		;WITH FORN_CTE AS (")
            sql.AppendLine("			SELECT")
            sql.AppendLine("				Id_Fornitore")
            sql.AppendLine("				, RifFornitore")
            sql.AppendLine("				, nr_Id = ROW_NUMBER() OVER (PARTITION BY Id_Fornitore ORDER BY Id_Fornitore)")
            sql.AppendLine("				, nr_Rif = ROW_NUMBER() OVER (PARTITION BY RifFornitore ORDER BY RifFornitore)")
            sql.AppendLine("			FROM (")
            sql.AppendLine("				SELECT z.Id_Fornitore, RifFornitore = IIF(CHARINDEX(':', z.RifFornitore) > 0, TRIM(SUBSTRING(z.RifFornitore, 1, CHARINDEX(':', z.RifFornitore) - 1)), TRIM(z.RifFornitore))")
            sql.AppendLine("				FROM @SXS sxs")
            sql.AppendLine("				INNER JOIN MeteoNT_StazioniXSensori sxs_o ON sxs.ID_SENSORE = sxs_o.Id_Sensore")
            sql.AppendLine("				INNER JOIN MeteoNT_Stazioni z ON z.Id = sxs_o.Id_Stazione AND z.FlagReale = 1")
            sql.AppendLine("			) T")
            sql.AppendLine("		)")
            sql.AppendLine()
            sql.AppendLine("		SELECT")
            sql.AppendLine("			@ID_FORNITORE = (SELECT TOP 1 Id_Fornitore FROM FORN_CTE ORDER BY nr_Id)")
            sql.AppendLine("			, @RIFFORNITORE = (SELECT TOP 1 RifFornitore FROM FORN_CTE ORDER BY nr_Rif)")
            sql.AppendLine()
            sql.AppendLine("		SELECT @DESCRIZIONE = Descrizione FROM MeteoNT_Fornitori WHERE Id = @ID_FORNITORE")
            sql.AppendLine()
            sql.AppendLine("		INSERT INTO MeteoNT_Stazioni (Id, Id_Fornitore, Nome, Lat_Dec, Lng_Dec, FlagReale, FlagSIM, RifFornitore)")
            sql.AppendLine("		VALUES (@ID_STAZIONE, @ID_FORNITORE, @STAZIONE, @LAT, @LNG, 0, 0, @RIFFORNITORE)")
            sql.AppendLine()
            sql.AppendLine("		INSERT INTO MeteoNT_VisibilitaStazioni (Id_Stazione, PIVA_Superuser, PIVA, Proprietario, DescrizioneAggiuntiva, Note, Anonima)")
            sql.AppendLine("		VALUES (@ID_STAZIONE, @PIVA_SUPERUSER, @PIVA, 1, @DESCRIZIONE, @NOTE, 0)")
            sql.AppendLine()
            sql.AppendLine("		SET @ALLOW_UPDATE = 1")
            sql.AppendLine()
            sql.AppendLine("		UPDATE @SXS SET ID_STAZIONE = @ID_STAZIONE")
            sql.AppendLine()
            sql.AppendLine("	END")
            sql.AppendLine("	ELSE")
            sql.AppendLine("	BEGIN")
            sql.AppendLine()
            sql.AppendLine("		SELECT @ALLOW_UPDATE = COALESCE(Proprietario, 0)")
            sql.AppendLine("		FROM MeteoNT_VisibilitaStazioni")
            sql.AppendLine("		WHERE Id_Stazione = @ID_STAZIONE AND PIVA_Superuser = @PIVA_SUPERUSER AND PIVA = @PIVA")
            sql.AppendLine()
            sql.AppendLine("		IF  @ALLOW_UPDATE = 1")
            sql.AppendLine("		BEGIN")
            sql.AppendLine("			UPDATE MeteoNT_Stazioni SET  Nome = @STAZIONE, Lat_Dec = @LAT, Lng_Dec = @LNG WHERE Id = @ID_STAZIONE")
            sql.AppendLine("		END")
            sql.AppendLine("		ELSE")
            sql.AppendLine("		BEGIN")
            sql.AppendLine("			SET @ID_STAZIONE = 0")
            sql.AppendLine("		END")
            sql.AppendLine("	END")
            sql.AppendLine()
            sql.AppendLine("	IF @ALLOW_UPDATE = 1")
            sql.AppendLine("	BEGIN")
            sql.AppendLine()
            sql.AppendLine("		DECLARE @SXS_OP TABLE (Id_Sensore INT, Op INT)")
            sql.AppendLine("		INSERT INTO @SXS_OP")
            sql.AppendLine("		SELECT Id_Sensore = COALESCE(x1.ID_SENSORE, x2.Id_Sensore), Op = IIF(x1.ID_SENSORE IS NOT NULL, IIF(x2.Id_Sensore IS NOT NULL, 0, 1), -1)")
            sql.AppendLine("		FROM @SXS x1")
            sql.AppendLine("		FULL OUTER JOIN (SELECT Id_Sensore FROM MeteoNT_StazioniXSensori WHERE Id_Stazione = @ID_STAZIONE) x2 ON x1.ID_SENSORE = x2.Id_Sensore")
            sql.AppendLine()
            sql.AppendLine("		--UPDATE")
            sql.AppendLine("		UPDATE x SET x.Ordine = x1.Ordine, x.Etichetta = x1.Etichetta, x.OutputConfig = x1.OutputConfig")
            sql.AppendLine("		FROM MeteoNT_StazioniXSensori x")
            sql.AppendLine("		INNER JOIN @SXS x1 ON x1.ID_STAZIONE = x.Id_Stazione AND x1.ID_SENSORE = x.Id_Sensore")
            sql.AppendLine("		INNER JOIN @SXS_OP o ON o.Id_Sensore = x1.ID_SENSORE AND o.Op = 0")
            sql.AppendLine()
            sql.AppendLine("		--INSERT")
            sql.AppendLine("		INSERT INTO MeteoNT_StazioniXSensori (Id_Stazione, Id_Sensore, FlagReale, Ordine, Etichetta, OutputConfig)")
            sql.AppendLine("		SELECT x.Id_Stazione, x.Id_Sensore, 0, x.Ordine, x.Etichetta, x.OutputConfig")
            sql.AppendLine("		FROM @SXS x")
            sql.AppendLine("		INNER JOIN @SXS_OP o ON o.Id_Sensore = x.ID_SENSORE AND o.Op = 1")
            sql.AppendLine()
            sql.AppendLine("		--DELETE")
            sql.AppendLine("		DELETE x")
            sql.AppendLine("		FROM MeteoNT_StazioniXSensori x")
            sql.AppendLine("		INNER JOIN @SXS_OP o ON o.Id_Sensore = x.Id_Sensore AND o.Op = -1")
            sql.AppendLine("		WHERE x.Id_Stazione = @ID_STAZIONE")
            sql.AppendLine()
            sql.AppendLine("	END")
            sql.AppendLine()
            sql.AppendLine("	COMMIT TRANSACTION")
            sql.AppendLine()
            sql.AppendLine("END TRY")
            sql.AppendLine("BEGIN CATCH")
            sql.AppendLine()
            sql.AppendLine("	ROLLBACK TRANSACTION")
            sql.AppendLine()
            sql.AppendLine("	SET @ID_STAZIONE = 0")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();")
            sql.AppendLine("    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();")
            sql.AppendLine("    DECLARE @ErrorState INT = ERROR_STATE();")
            sql.AppendLine("    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);")
            sql.AppendLine()
            sql.AppendLine("END CATCH")
            sql.AppendLine()
            sql.AppendLine("SELECT ID_STAZIONE_AGGIORNATA = @ID_STAZIONE")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Id_Stazione = DT.Rows(0)("ID_STAZIONE_AGGIORNATA")

        Catch ex As Exception

            Id_Stazione = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return Id_Stazione
    End Function


    Public Function LeggiVisibilitaStazione(Id_Stazione As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiVisibilitaStazione"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT PIVA_Superuser, PIVA, Proprietario, FlagReale ")
            sql.AppendLine("FROM MeteoNT_Stazioni staz ")
            sql.AppendLine("INNER JOIN MeteoNT_VisibilitaStazioni vis ON vis.Id_Stazione = staz.Id ")
            sql.AppendLine("WHERE staz.Id = " & Id_Stazione)

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return DT
    End Function


    Public Function EliminaStazione(Id_Stazione As Integer, ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "MeteoNT.EliminaStazione"
        Dim xRisp As Boolean = False

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ID_STAZIONE AS INTEGER = " & Id_Stazione)
            sql.AppendLine("DECLARE @FLAG_OK AS BIT = 1 ")
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("    BEGIN TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("	DELETE FROM MeteoNT_VisibilitaStazioni WHERE Id_Stazione = @ID_STAZIONE ")
            sql.AppendLine("	DELETE FROM MeteoNT_StazioniXSensori WHERE Id_Stazione = @ID_STAZIONE ")
            sql.AppendLine("	DELETE FROM MeteoNT_Stazioni WHERE Id = @ID_STAZIONE ")
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


    Public Function LeggiQuadrantiConDistanza(ByVal utm_x As Integer, ByVal utm_y As Integer, ByVal limiteDistanza As Integer, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiQuadrantiConDistanza"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql.Clear()

            sql.AppendLine("DECLARE @X FLOAT = " & utm_x)
            sql.AppendLine("DECLARE @Y FLOAT = " & utm_y)
            sql.AppendLine()
            sql.AppendLine("SELECT TOP 10 ID_Quadrante, Quadrante_Des, X_UTM, Y_UTM, Altitudine, Distanza ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("        ID_Quadrante")
            sql.AppendLine("        , Quadrante_Des ")
            sql.AppendLine("        , X_UTM ")
            sql.AppendLine("        , Y_UTM ")
            sql.AppendLine("        , Altitudine ")
            sql.AppendLine("        , SQRT(POWER((X_UTM - @X), 2) + POWER( (Y_UTM - @Y ), 2) ) AS Distanza ")
            sql.AppendLine("	FROM TB_Quadranti ")
            sql.AppendLine(") T ")
            If limiteDistanza > 0 Then
                sql.AppendLine("WHERE Distanza <= " & limiteDistanza)
            End If
            sql.AppendLine("ORDER BY Distanza ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT
    End Function


    Public Function LeggiInfoStazione(piva_superuser As String, piva As String, TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiInfoStazione"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

                    Dim srcTableDati As String = "MeteoNT_DatiSorgente"

                    If TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

                        srcTableDati = "MeteoNT_DatiOrari"
                    End If

                    sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
                    sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
                    sql.AppendLine($"DECLARE @ID INTEGER = {IdStazione}")
                    sql.AppendLine()
                    sql.AppendLine("SELECT Stazione_Des = Nome, UltimoAggiornamento")
                    sql.AppendLine("FROM (")
                    sql.AppendLine("	SELECT Id, Nome")
                    sql.AppendLine("	FROM MeteoNT_Stazioni")
                    sql.AppendLine("	WHERE Categoria <> 1")
                    sql.AppendLine("	UNION ALL")
                    sql.AppendLine("	SELECT Id, Nome = IIF(a.Id_Stazione IS NULL, z.Nome, a.Alias)")
                    sql.AppendLine("	FROM MeteoNT_Stazioni z")
                    sql.AppendLine("	LEFT JOIN MeteoNT_VisibilitaStazioniAlias a ON a.Id_Stazione = z.Id AND a.PIVA_Superuser = @PIVA_SUPERUSER AND a.PIVA = @PIVA")
                    sql.AppendLine("	WHERE Categoria = 1")
                    sql.AppendLine(") s")
                    sql.AppendLine("INNER JOIN (")
                    sql.AppendLine("	SELECT z.Id, MAX(DataOra) AS UltimoAggiornamento")
                    sql.AppendLine("	FROM MeteoNT_Stazioni z")
                    sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Stazione = z.Id")
                    sql.AppendLine($"	INNER JOIN {srcTableDati} dati ON dati.Id_Sensore = sxs.Id_Sensore")
                    sql.AppendLine("	WHERE z.Id = @ID")
                    sql.AppendLine("	GROUP BY z.Id")
                    sql.AppendLine(") d ON d.Id = s.Id")

                Case enum_Meteo_Tiposorgente.Gias_RER

                    sql.AppendLine("SELECT Stazione_Des, UltimoAggiornamento ")
                    sql.AppendLine("FROM TB_Stazioni s ")
                    sql.AppendLine("INNER JOIN ( ")
                    sql.AppendLine("	SELECT st.ID_Stazione, MAX(Tempo) AS UltimoAggiornamento ")
                    sql.AppendLine("	FROM TB_Stazioni st ")
                    sql.AppendLine("	INNER JOIN Dati_Meteo_SHH dati ON dati.ID_Stazione = st.ID_Stazione ")
                    sql.AppendLine("	WHERE st.ID_Stazione = " & IdStazione.ToString())
                    sql.AppendLine("	GROUP BY st.ID_Stazione ")
                    sql.AppendLine(") d ON d.ID_Stazione = s.ID_Stazione ")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sql.AppendLine("SELECT Quadrante_Des AS Stazione_Des, UltimoAggiornamento ")
                    sql.AppendLine("FROM TB_Quadranti q ")
                    sql.AppendLine("INNER JOIN ( ")
                    sql.AppendLine("	SELECT q.ID_Quadrante, MAX(Tempo) AS UltimoAggiornamento ")
                    sql.AppendLine("	FROM TB_Quadranti q ")
                    sql.AppendLine("	INNER JOIN Dati_Meteo_QHH dati ON dati.ID_Quadrante = q.ID_Quadrante ")
                    sql.AppendLine("	WHERE q.ID_Quadrante = " & IdStazione.ToString())
                    sql.AppendLine("	GROUP BY q.ID_Quadrante ")
                    sql.AppendLine(") d ON d.ID_Quadrante = q.ID_Quadrante ")

            End Select

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

    Public Function LeggiElencoStazioni(piva_superuser As String, piva As String, TipoSorgente As enum_Meteo_Tiposorgente, CodiciStazione() As Integer, FlagSensori As Boolean, Separatore As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiElencoStazioni"
        Dim DT As DataTable = Nothing

        Try
            Dim sql As New StringBuilder

            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine()

            Dim sqlStaz As New StringBuilder

            sqlStaz.AppendLine("SELECT ")
            sqlStaz.AppendLine($"    TipoSorgente = {CInt(TipoSorgente)}")
            sqlStaz.AppendLine("    , STAZ.* ")
            sqlStaz.AppendLine("FROM ( ")

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali

                    sqlStaz.AppendLine("	SELECT ")
                    sqlStaz.AppendLine("		Id_Stazione = S.Id ")
                    sqlStaz.AppendLine("		, Nome_Stazione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sqlStaz.AppendLine("		, Fornitore = F.Descrizione ")
                    sqlStaz.AppendLine("		, RifFornitore ")
                    sqlStaz.AppendLine("		, FlagReale ")
                    sqlStaz.AppendLine("	FROM MeteoNT_Stazioni S ")
                    sqlStaz.AppendLine("	INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sqlStaz.AppendLine("	INNER JOIN ( ")
                    sqlStaz.AppendLine("		SELECT Id_Stazione, DescrizioneAggiuntiva, Anonima FROM ")
                    sqlStaz.AppendLine("		( ")
                    sqlStaz.AppendLine("			SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Stazione ORDER BY Id_Stazione ASC, Proprietario DESC) AS Cnt ")
                    sqlStaz.AppendLine("	        FROM ( ")
                    sqlStaz.AppendLine("				SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sqlStaz.AppendLine("				FROM MeteoNT_VisibilitaStazioni ")
                    sqlStaz.AppendLine("				WHERE PIVA_Superuser = @PIVA_SUPERUSER AND PIVA = @PIVA")
                    sqlStaz.AppendLine()
                    sqlStaz.AppendLine("				UNION ")
                    sqlStaz.AppendLine()
                    sqlStaz.AppendLine("				SELECT Id_Stazione, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sqlStaz.AppendLine("				FROM MeteoNT_VisibilitaStazioni ")
                    sqlStaz.AppendLine("				WHERE PIVA_Superuser = @PIVA_SUPERUSER AND PIVA = '*' ")
                    sqlStaz.AppendLine("		    ) TBL1 ")

                    If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali Then
                        sqlStaz.AppendLine("            WHERE Proprietario = 1 --se solo stazioni aziendali ")
                    End If

                    sqlStaz.AppendLine("	    ) TBL2 ")
                    sqlStaz.AppendLine("	    WHERE Cnt = 1 ")
                    sqlStaz.AppendLine("	) vis on vis.Id_Stazione = S.Id ")

                Case enum_Meteo_Tiposorgente.Gias_RER

                    sqlStaz.AppendLine("    SELECT ")
                    sqlStaz.AppendLine("    	Id_Stazione = S.ID_Stazione ")
                    sqlStaz.AppendLine("    	, Nome_Stazione = Stazione_Des ")
                    sqlStaz.AppendLine("        , Fornitore = 'Stazione RER' ")
                    sqlStaz.AppendLine("        , RifFornitore = '' ")
                    sqlStaz.AppendLine("        , FlagReale = CAST(1 AS BIT) ")
                    sqlStaz.AppendLine("    FROM TB_Stazioni S ")
                    sqlStaz.AppendLine("    INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
                    sqlStaz.AppendLine("    INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sqlStaz.AppendLine("    SELECT ")
                    sqlStaz.AppendLine("    	Id_Stazione = Q.ID_Quadrante ")
                    sqlStaz.AppendLine("    	, Nome_Stazione = Quadrante_Des ")
                    sqlStaz.AppendLine("        , Fornitore = 'Quadrante RER' ")
                    sqlStaz.AppendLine("        , RifFornitore = '' ")
                    sqlStaz.AppendLine("        , FlagReale = CAST(1 AS BIT) ")
                    sqlStaz.AppendLine("    FROM TB_Quadranti Q ")

                Case enum_Meteo_Tiposorgente.Pubbliche

                    sqlStaz.AppendLine("	SELECT")
                    sqlStaz.AppendLine("		Id_Stazione = S.Id")
                    sqlStaz.AppendLine("		, Nome_Stazione = IIF(A.Id_Stazione IS NULL, F.Descrizione + ' (' + S.Nome + ')', A.Alias)")
                    sqlStaz.AppendLine("		, Fornitore = F.Descrizione")
                    sqlStaz.AppendLine("		, RifFornitore")
                    sqlStaz.AppendLine("		, FlagReale")
                    sqlStaz.AppendLine("	FROM MeteoNT_Stazioni S")
                    sqlStaz.AppendLine("	INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore")
                    sqlStaz.AppendLine("	LEFT JOIN MeteoNT_VisibilitaStazioniAlias A ON A.Id_Stazione = s.Id AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA")
                    sqlStaz.AppendLine("	WHERE Categoria = 1")

            End Select

            sqlStaz.AppendLine(") STAZ ")
            sqlStaz.AppendLine("WHERE STAZ.Id_Stazione IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", CodiciStazione)) & ") ")

            If Not FlagSensori Then

                sql.AppendLine(sqlStaz.ToString)
            Else

                sql.AppendLine("SELECT ")
                sql.AppendLine("   STAZIONI.* ")

                If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali OrElse
                    TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner OrElse
                    TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

                    sql.AppendLine("   , SENSORI.Sensori ")

                Else

                    sql.AppendLine("   , Sensori = '' ")

                End If

                sql.AppendLine("FROM ( ")
                sql.AppendLine()
                sql.AppendLine(sqlStaz.ToString())
                sql.AppendLine()
                sql.AppendLine(") STAZIONI ")

                If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali OrElse
                    TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner OrElse
                    TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

                    sql.AppendLine("INNER JOIN ( ")
                    sql.AppendLine()
                    sql.AppendLine()
                    sql.AppendLine()
                    sql.AppendLine("SELECT Id_Stazione, Sensori = STRING_AGG(Sensore, '" & Separatore & "') ")
                    sql.AppendLine("FROM ( ")
                    sql.AppendLine("	SELECT Id_Stazione, Sensore = COALESCE(sxs.Etichetta, sens.Etichetta, '') ")
                    sql.AppendLine("	FROM MeteoNT_StazioniXSensori sxs ")
                    sql.AppendLine("	INNER JOIN MeteoNT_Sensori sens ON sens.Id = sxs.Id_Sensore ")
                    sql.AppendLine("	WHERE sxs.Id_Stazione IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", CodiciStazione)) & ") ")
                    sql.AppendLine("	ORDER BY Ordine OFFSET 0 ROWS ")
                    sql.AppendLine(") T ")
                    sql.AppendLine("GROUP BY Id_Stazione ")
                    sql.AppendLine()
                    sql.AppendLine()
                    sql.AppendLine()
                    sql.AppendLine(") SENSORI ON SENSORI.Id_Stazione = STAZIONI.Id_Stazione ")

                End If

            End If

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function




    Public Function LeggiElencoAlias(piva_superuser As String, piva As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.LeggiElencoAlias"

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine()
            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine()
            sql.AppendLine("SELECT Id, Lat = Lat_Dec, Lng = Lng_Dec, [Name] = Alias")
            sql.AppendLine("FROM MeteoNT_Stazioni z WITH (NOLOCK)")
            sql.AppendLine("INNER JOIN MeteoNT_VisibilitaStazioniAlias a ON a.Id_Stazione = z.Id AND a.PIVA_Superuser = @PIVA_SUPERUSER AND a.PIVA = @PIVA")

            Dim dt = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Return dt
        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try
    End Function



    Public Function LeggiStazioneXAlias(piva_superuser As String, piva As String, lat As Decimal, lng As Decimal, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiStazioneXAlias"

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine($"DECLARE @SRCPOINT AS GEOGRAPHY = GEOGRAPHY::Point({Convert.ToString(lat, CultureInfo.InvariantCulture)}, {Convert.ToString(lng, CultureInfo.InvariantCulture)}, 4326)")
            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine()
            sql.AppendLine("SELECT Id, Lat, Lng, [Name], Fornitore, Dist, FlagNew = IIF(A.Id_Stazione IS NULL, 1, 0), A.Alias")
            sql.AppendLine("FROM (")
            sql.AppendLine("	SELECT TOP 1 z.Id, Lat = Lat_Dec, Lng = Lng_Dec, [Name] = z.Nome, Fornitore = f.Descrizione, Dist = GeoEntity.STDistance(@SRCPOINT)")
            sql.AppendLine("	FROM MeteoNT_Stazioni z WITH (NOLOCK)")
            sql.AppendLine("	INNER JOIN MeteoNT_Fornitori f WITH (NOLOCK) ON f.Id = z.Id_Fornitore")
            sql.AppendLine("	WHERE Categoria = 1")
            sql.AppendLine("	ORDER BY Dist")
            sql.AppendLine(") T")
            sql.AppendLine("LEFT JOIN MeteoNT_VisibilitaStazioniAlias A ON A.Id_Stazione = T.Id AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA")

            Dim dt = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Return dt
        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try
    End Function



    Public Function UpsertAlias(piva_superuser As String, piva As String, id As Integer, strAlias As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.UpsertAlias"

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine($"DECLARE @ID INTEGER = {id}")
            sql.AppendLine($"DECLARE @ALIAS AS VARCHAR(MAX) = '{strAlias}'")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO MeteoNT_VisibilitaStazioniAlias AS t")
            sql.AppendLine("USING")
            sql.AppendLine("	(SELECT PIVA_Superuser = @PIVA_SUPERUSER, PIVA = @PIVA, Id_Stazione = @ID, Alias = @ALIAS) AS s")
            sql.AppendLine("ON t.PIVA_Superuser = s.PIVA_Superuser AND t.PIVA = s.PIVA AND t.Id_Stazione = s.Id_Stazione")
            sql.AppendLine("WHEN MATCHED THEN")
            sql.AppendLine("	UPDATE SET t.Alias = s.Alias")
            sql.AppendLine("WHEN NOT MATCHED THEN")
            sql.AppendLine("	INSERT (PIVA_Superuser, PIVA, Id_Stazione, Alias)")
            sql.AppendLine("	VALUES (s.PIVA_Superuser, s.PIVA, s.Id_Stazione, s.Alias);")
            sql.AppendLine()
            sql.AppendLine("SELECT Id, Lat = Lat_Dec, Lng = Lng_Dec, [Name] = Alias")
            sql.AppendLine("FROM MeteoNT_Stazioni z WITH (NOLOCK)")
            sql.AppendLine("INNER JOIN MeteoNT_VisibilitaStazioniAlias a ON a.Id_Stazione = z.Id")
            sql.AppendLine("WHERE a.Id_Stazione = @ID AND a.PIVA_Superuser = @PIVA_SUPERUSER AND a.PIVA = @PIVA")

            Dim dt = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Return dt
        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try

    End Function



    Public Function DeleteAlias(piva_superuser As String, piva As String, id As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.DeleteAlias"

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine($"DECLARE @PIVA_SUPERUSER VARCHAR(50) = '{piva_superuser}'")
            sql.AppendLine($"DECLARE @PIVA VARCHAR(50) = '{piva}'")
            sql.AppendLine($"DECLARE @ID INTEGER = {id}")
            sql.AppendLine()
            sql.AppendLine("DELETE FROM MeteoNT_VisibilitaStazioniAlias")
            sql.AppendLine("WHERE PIVA_Superuser = @PIVA_Superuser AND PIVA = @PIVA AND Id_Stazione = @ID")
            sql.AppendLine()
            sql.AppendLine("SELECT Deleted = @@ROWCOUNT")

            Dim dt = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Return dt
        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try

    End Function


    Public Class IdLatLng
        Public Id As Integer
        Public Lat As Decimal
        Public Lng As Decimal
        Public Id_Stazione As Integer
    End Class

    Public Function StazioniPubblicheDaLatLng(coords As List(Of IdLatLng), ByVal ObjParametri_Server As AgronicaCoreParametri) As List(Of IdLatLng)

        Dim NomeRoutine As String = "MeteoNT.StazioniPubblicheDaLatLng"

        Dim outlist As New List(Of IdLatLng)

        Try
            Dim inRows As New List(Of String)

            For Each c In coords

                inRows.Add("(" &
                       Convert.ToString(c.Id, Globalization.CultureInfo.InvariantCulture) & ", " &
                       Convert.ToString(c.Lat, Globalization.CultureInfo.InvariantCulture) & ", " &
                       Convert.ToString(c.Lng, Globalization.CultureInfo.InvariantCulture) &
                       ")")
            Next

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @COORD_TBL TABLE(Id INT, Lat REAL, Lng REAL) ")
            sql.AppendLine("INSERT INTO @COORD_TBL VALUES ")
            sql.AppendLine(String.Join(", " & vbCrLf, inRows))
            sql.AppendLine()
            sql.AppendLine("SELECT C.Id, C.Lat, C.Lng, Id_Stazione = Z.Id")
            sql.AppendLine("FROM MeteoNT_Stazioni Z WITH(NOLOCK) ")
            sql.AppendLine("INNER JOIN @COORD_TBL C ON GEOGRAPHY::Point(C.Lat, C.Lng, 4326).STIntersects(Z.GeoEntity) = 1 ")
            sql.AppendLine("WHERE Categoria = 1 ")

            Dim dt = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            If dt IsNot Nothing Then

                For Each row In dt.Rows

                    outlist.Add(New IdLatLng With {
                                .Id = row("Id"),
                                .Lat = row("Lat"),
                                .Lng = row("Lng"),
                                .Id_Stazione = row("Id_Stazione")
                                })
                Next
            End If
        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return outlist
    End Function

    Public Function GetDataUltimaStoricizzazioneMeteoNT(ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.GetDataUltimaStoricizzazioneMeteoNT"
        Dim Riepilogo As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT TOP(1) DataUltimaStoricizzazione FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[LogEventi]")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function

    Public Function StoricizzazioneMeteoNT(ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.StoricizzazioneMeteoNT"
        Dim Riepilogo As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("-- TipiSensore")
            sql.AppendLine()
            sql.AppendLine("DECLARE @NewTipiSensore TABLE(Id int, Tipo VARCHAR(50), UM VARCHAR(10), FunAggreg VARCHAR(50), Misura VARCHAR(MAX), DefaultOutputConfig VARCHAR(MAX), IsUpdated bit)")
            sql.AppendLine("DECLARE @UpdatedTipiSensore TABLE(Id int)")
            sql.AppendLine("    INSERT INTO @NewTipiSensore")
            sql.AppendLine("SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_TipiSensore]")
            sql.AppendLine("DECLARE @RESULT_TOTAL_TIPISENSORE INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_TIPISENSORE INT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("-- Fornitori")
            sql.AppendLine()
            sql.AppendLine("DECLARE @NewFornitori TABLE(Id int, Descrizione VARCHAR(MAX), IsUpdated bit)")
            sql.AppendLine("DECLARE @UpdatedFornitori TABLE(Id int)")
            sql.AppendLine("INSERT INTO @NewFornitori")
            sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Fornitori]")
            sql.AppendLine("DECLARE @RESULT_TOTAL_FORNITORI INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_FORNITORI INT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("-- Sensori")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewSensori"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpSensoriUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpNewSensori(Id int, Id_TipoSensore int, Etichetta VARCHAR(MAX), ChiaveImport VARCHAR(100), IsUpdated bit)")
            sql.AppendLine("CREATE TABLE #TmpSensoriUpdated(Id int)")
            sql.AppendLine("INSERT INTO #TmpNewSensori")
            sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Sensori]")
            sql.AppendLine("DECLARE @RESULT_TOTAL_SENSORI INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_SENSORI INT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("-- Stazioni")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewStazioni"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpStazioniUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpNewStazioni(Id int, Id_Fornitore int, Nome VARCHAR(MAX), Lat_Dec FLOAT, Lng_Dec FLOAT, Altitudine FLOAT, FlagReale BIT, FlagSIM BIT, RifFornitore VARCHAR(MAX), SystemTimeZoneInfo VARCHAR(MAX), IsUpdated bit)")
            sql.AppendLine("CREATE TABLE #TmpStazioniUpdated(Id int)")
            sql.AppendLine("INSERT INTO #TmpNewStazioni")
            sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Stazioni]")
            sql.AppendLine("DECLARE @RESULT_TOTAL_STAZIONI INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_STAZIONI INT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("-- StazioniXSensori")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewStazioniXSensori"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpStazioniXSensoriUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpNewStazioniXSensori(Id_Stazione int, Id_Sensore int, FlagReale BIT, OutputConfig VARCHAR(MAX), Ordine INT, Etichetta VARCHAR(MAX), IsUpdated bit)")
            sql.AppendLine("CREATE TABLE #TmpStazioniXSensoriUpdated(Id_Stazione int, Id_Sensore int)")
            sql.AppendLine("INSERT INTO #TmpNewStazioniXSensori")
            sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_StazioniXSensori]")
            sql.AppendLine("DECLARE @RESULT_TOTAL_STAZIONIXSENSORI INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_STAZIONIXSENSORI INT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("-- DatiOrari")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewDatiOrari"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiOrariUpdated"))
            sql.AppendLine()
            sql.AppendLine("DECLARE @LastYearFirstDay DATETIME = CAST((DATEADD(yy, -1, DATEADD(d, -1 * DATEPART(dd, getdate()) + 1, DATEADD(MM, -1 * DATEPART(MM, getdate()) + 1, GETDATE())))) as date)")
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpNewDatiOrari(Id_Sensore int, DataOra DATETIME, Valore Real, Confidenza SMALLINT, IsUpdated bit, UNIQUE CLUSTERED (Id_Sensore, DataOra))")
            sql.AppendLine("CREATE TABLE #TmpDatiOrariUpdated(Id_Sensore int, DataOra DATETIME, UNIQUE CLUSTERED (Id_Sensore, DataOra))")
            sql.AppendLine("INSERT INTO #TmpNewDatiOrari")
            sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_DatiOrari] ")
            sql.AppendLine("    WHERE DataOra < @LastYearFirstDay AND Confidenza > 0 ")
            sql.AppendLine("DECLARE @RESULT_TOTAL_DATIORARI BIGINT = ROWCOUNT_BIG(); ")
            sql.AppendLine("DECLARE @RESULT_INSERT_DATIORARI BIGINT = 0;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE FORNITORI")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.Descrizione = s.Descrizione")
            sql.AppendLine("    OUTPUT INSERTED.Id INTO @UpdatedFornitori(Id)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Fornitori] t")
            sql.AppendLine("    INNER JOIN @NewFornitori s ON s.Id = t.Id")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_FORNITORI INT = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_FORNITORI < @RESULT_TOTAL_FORNITORI) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM @NewFornitori s")
            sql.AppendLine("        INNER JOIN @UpdatedFornitori u ON u.Id = s.Id;")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Fornitori](Id, Descrizione)")
            sql.AppendLine("        SELECT Id, Descrizione FROM @NewFornitori")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_FORNITORI = @@ROWCOUNT")
            sql.AppendLine("    END;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE TIPI SENSORE")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.Tipo = s.Tipo, t.UM = s.UM, t.FunAggreg = s.FunAggreg, t.Misura = s.Misura, t.DefaultOutputConfig = s.DefaultOutputConfig")
            sql.AppendLine("    OUTPUT INSERTED.Id INTO @UpdatedTipiSensore(s.Id)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_TipiSensore] t")
            sql.AppendLine("    INNER JOIN @NewTipiSensore s ON s.Id = t.Id")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_TIPISENSORE INT = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_TIPISENSORE < @RESULT_TOTAL_TIPISENSORE) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM @NewTipiSensore s")
            sql.AppendLine("        INNER JOIN @UpdatedTipiSensore u ON u.Id = s.Id;")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_TipiSensore]")
            sql.AppendLine("        SELECT Id, Tipo, UM, FunAggreg, Misura, DefaultOutputConfig FROM @NewTipiSensore")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_TIPISENSORE = @@ROWCOUNT")
            sql.AppendLine("    END;")
            sql.AppendLine("    ")
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE SENSORI")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.Id_TipoSensore = s.Id_TipoSensore, t.Etichetta = s.Etichetta, t.ChiaveImport = s.ChiaveImport")
            sql.AppendLine("    OUTPUT INSERTED.Id INTO #TmpSensoriUpdated(s.Id)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Sensori] t")
            sql.AppendLine("    INNER JOIN #TmpNewSensori s ON s.Id = t.Id")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_SENSORI INT = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_SENSORI < @RESULT_TOTAL_SENSORI) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM #TmpNewSensori s")
            sql.AppendLine("        INNER JOIN #TmpSensoriUpdated u ON u.Id = s.Id;")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Sensori]")
            sql.AppendLine("        SELECT Id, Id_TipoSensore, Etichetta, ChiaveImport FROM #TmpNewSensori")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_SENSORI = @@ROWCOUNT")
            sql.AppendLine("    END;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE STAZIONI")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.Id_Fornitore = s.Id_Fornitore, t.Nome = s.Nome, t.Lat_Dec = s.Lat_Dec, t.Lng_Dec = s.Lng_Dec, t.Altitudine = s.Altitudine, t.FlagReale = s.FlagReale, t.FlagSIM = s.FlagSIM, t.RifFornitore = s.RifFornitore, t.SystemTimeZoneInfo = s.SystemTimeZoneInfo")
            sql.AppendLine("    OUTPUT INSERTED.Id INTO #TmpStazioniUpdated(s.Id)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Stazioni] t")
            sql.AppendLine("    INNER JOIN #TmpNewStazioni s ON s.Id = t.Id")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_STAZIONI INT = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_STAZIONI < @RESULT_TOTAL_STAZIONI) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM #TmpNewStazioni s")
            sql.AppendLine("        INNER JOIN #TmpStazioniUpdated u ON u.Id = s.Id;")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Stazioni]")
            sql.AppendLine("        SELECT Id, Id_Fornitore, Nome, Lat_Dec, Lng_Dec, Altitudine, FlagReale, FlagSIM, RifFornitore, SystemTimeZoneInfo FROM #TmpNewStazioni")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_STAZIONI = @@ROWCOUNT")
            sql.AppendLine("    END;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE STAZIONIXSENSORI")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.FlagReale = s.FlagReale, t.OutputConfig = s.OutputConfig, t.Ordine = s.Ordine, t.Etichetta = s.Etichetta")
            sql.AppendLine("    OUTPUT INSERTED.Id_Stazione, INSERTED.Id_Sensore INTO #TmpStazioniXSensoriUpdated(Id_Stazione, Id_Sensore)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_StazioniXSensori] t")
            sql.AppendLine("    INNER JOIN #TmpNewStazioniXSensori s ON s.Id_Stazione = t.Id_Stazione AND s.Id_Sensore = t.Id_Sensore")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_STAZIONIXSENSORI INT = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_STAZIONIXSENSORI < @RESULT_TOTAL_STAZIONIXSENSORI) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM #TmpNewStazioniXSensori s")
            sql.AppendLine("        INNER JOIN #TmpStazioniXSensoriUpdated u ON u.Id_Stazione = s.Id_Stazione AND u.Id_Sensore = s.Id_Sensore;")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_StazioniXSensori]")
            sql.AppendLine("        SELECT Id_Stazione, Id_Sensore, FlagReale, OutputConfig, Ordine, Etichetta FROM #TmpNewStazioniXSensori")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_STAZIONIXSENSORI = @@ROWCOUNT")
            sql.AppendLine("    END;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    UPDATE DATIORARI")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE t")
            sql.AppendLine("    SET t.Valore = s.Valore, t.Confidenza = s.Confidenza")
            sql.AppendLine("    OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpDatiOrariUpdated(Id_Sensore, DataOra)")
            sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_DatiOrari] t")
            sql.AppendLine("    INNER JOIN #TmpNewDatiOrari s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra")
            sql.AppendLine()
            sql.AppendLine("    DECLARE @RESULT_UPDATE_DATIORARI INT = ROWCOUNT_BIG()")
            sql.AppendLine()
            sql.AppendLine("    IF (@RESULT_UPDATE_DATIORARI < @RESULT_TOTAL_DATIORARI) -- if all rows were updates then skip, else insert remaining")
            sql.AppendLine("    BEGIN")
            sql.AppendLine()
            sql.AppendLine("        UPDATE s")
            sql.AppendLine("        SET s.IsUpdated = 1")
            sql.AppendLine("        FROM #TmpNewDatiOrari s")
            sql.AppendLine("        INNER JOIN #TmpDatiOrariUpdated u ON (u.Id_Sensore = s.Id_Sensore AND u.DataOra = s.DataOra);")
            sql.AppendLine()
            sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_DatiOrari]")
            sql.AppendLine("        SELECT Id_Sensore, DataOra, Valore, Confidenza FROM #TmpNewDatiOrari")
            sql.AppendLine("        WHERE IsUpdated = 0;")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_DATIORARI = ROWCOUNT_BIG()")
            sql.AppendLine("    END;")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    Delete DatiOrari")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    DELETE FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_DatiOrari]")
            sql.AppendLine("        WHERE DataOra < @LastYearFirstDay")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/********************************************************************************************************************************************")
            sql.AppendLine("    Update data ultima storicizzazione")
            sql.AppendLine("*********************************************************************************************************************************************/")
            sql.AppendLine()
            sql.AppendLine("    UPDATE [ZZ_AgronicaMeteoSuite_Storico].[dbo].[LogEventi] SET DataUltimaStoricizzazione = GETDATE()")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("END TRY")
            sql.AppendLine("BEGIN CATCH")
            sql.AppendLine("    IF @@TRANCOUNT > 0")
            sql.AppendLine("        BEGIN;")
            sql.AppendLine("            ROLLBACK")
            sql.AppendLine("        END;")
            sql.AppendLine("END CATCH;")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewSensori"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpSensoriUpdated"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewStazioni"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpStazioniUpdated"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewStazioniXSensori"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpStazioniXSensoriUpdated"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpNewDatiOrari"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiOrariUpdated"))
            sql.AppendLine()
            sql.AppendLine("--Ritorno un riepilogo ")
            sql.AppendLine()
            sql.AppendLine("SELECT T.* FROM (VALUES ")
            sql.AppendLine("('MeteoNT_Fornitori', @RESULT_TOTAL_FORNITORI, @RESULT_UPDATE_FORNITORI, @RESULT_INSERT_FORNITORI),")
            sql.AppendLine("('MeteoNT_TipiSensore', @RESULT_TOTAL_TIPISENSORE, @RESULT_UPDATE_TIPISENSORE, @RESULT_INSERT_TIPISENSORE), ")
            sql.AppendLine("('MeteoNT_Sensori', @RESULT_TOTAL_SENSORI, @RESULT_UPDATE_SENSORI, @RESULT_INSERT_SENSORI),")
            sql.AppendLine("('MeteoNT_Stazioni', @RESULT_TOTAL_STAZIONI, @RESULT_UPDATE_STAZIONI, @RESULT_INSERT_STAZIONI),")
            sql.AppendLine("('MeteoNT_StazioniXSensori', @RESULT_TOTAL_STAZIONIXSENSORI, @RESULT_UPDATE_STAZIONIXSENSORI, @RESULT_INSERT_STAZIONIXSENSORI),")
            sql.AppendLine("('MeteoNT_DatiOrari', @RESULT_TOTAL_DATIORARI, @RESULT_UPDATE_DATIORARI, @RESULT_INSERT_DATIORARI)")
            sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]);")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function

    Public Class TblSensoriSorgente
        Public ReadOnly NomeTabella As String
        Public ReadOnly CampoId As String
        Public ReadOnly ChiaveImport As String
        Public ReadOnly CampoIdStazione As String
        Public Sub New(_tabella As String, _id As String, _chiave As String, _IdStazione As String)
            NomeTabella = _tabella
            CampoId = _id
            ChiaveImport = _chiave
            CampoIdStazione = _IdStazione
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

    Public Function EliminaDatiSorgenteByStazioneId(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal stazioneId As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "MeteoNT.EliminaDatiSorgente"
        Dim RetValue As Boolean = True

        Dim Riepilogo As DataTable

        Console.WriteLine(tblDati)

        Try
            Dim sql As New StringBuilder

            sql.AppendLine("DECLARE @dataEliminazione DATETIME = (")
            sql.AppendLine("    SELECT TOP(1) d." & tblDati.CampoDataOra & " FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id")
            sql.AppendLine("    WHERE s." & tblSensori.CampoIdStazione & " = " & stazioneId)
            sql.AppendLine("    AND Elaborato = 0 AND DATEDIFF(month, d." & tblDati.CampoDataOra & ", GETDATE()) > 2")
            sql.AppendLine("    ORDER BY d." & tblDati.CampoDataOra & " asc);")
            sql.AppendLine()
            sql.AppendLine("DELETE d FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id")
            sql.AppendLine("WHERE s." & tblSensori.CampoIdStazione & " = " & stazioneId)
            sql.AppendLine("AND Elaborato = 1 AND s.Trasferibile = 1 AND d." & tblDati.CampoDataOra & " < COALESCE(DATEADD(HOUR, DATEDIFF(HOUR, 0, (@dataEliminazione)), 0), (DATEADD(mm, -2, DATEADD(HOUR, DATEDIFF(HOUR, 0, (GETDATE())), 0))));")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function

    Public Function ScriviDatiOrari(tblSensori As TblSensoriSorgente, tblDati As TblDatiSorgente, confidenza As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.ScriviDatiOrari"

        Dim Riepilogo As DataTable

        Try

            Dim sql As New StringBuilder

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()

            sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpDatiSorgente (Src_Id INT NOT NULL, Id_Stazione INT NOT NULL, Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, IsUpdate BIT NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiSorgente ON #TmpDatiSorgente (Id_Stazione, Id_Sensore, DataOra); ")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpUpdated (Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpUpdated ON  #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO #TmpDatiSorgente (Src_Id, Id_Stazione, Id_Sensore, DataOra, Valore, IsUpdate) ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("    T.Src_Id ")
            sql.AppendLine("    , T.Id_Stazione ")
            sql.AppendLine("    , T.Id_Sensore ")
            sql.AppendLine("    , DataOra = CASE WHEN D.UTC = 1 THEN CAST(D.DataOra AT TIME ZONE 'UTC' AT TIME ZONE T.SystemTimeZoneInfo AS DATETIME) ELSE D.DataOra END ")
            sql.AppendLine("    , D.Valore ")
            sql.AppendLine("    , 0 ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		Src_Id = src.Id ")
            sql.AppendLine("        , staz.Id AS Id_Stazione ")
            sql.AppendLine("        , sens.Id AS Id_Sensore ")
            sql.AppendLine("        , SystemTimeZoneInfo = COALESCE(staz.SystemTimeZoneInfo, 'W. Europe Standard Time') ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine()
            sql.AppendLine("		SELECT " & tblSensori.CampoId & ", ChiaveImport = " & tblSensori.ChiaveImport)
            sql.AppendLine("		FROM " & tblSensori.NomeTabella)
            sql.AppendLine()
            sql.AppendLine("	) src ")
            sql.AppendLine("    INNER JOIN MeteoNT_Sensori			sens	ON sens.ChiaveImport = src.ChiaveImport ")
            sql.AppendLine("    INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
            sql.AppendLine("    INNER JOIN MeteoNT_Stazioni			staz	ON staz.Id = sxs.Id_Stazione ")
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
            sql.AppendLine("		, Id_Stazione ")
            sql.AppendLine("		, Id_Sensore ")
            sql.AppendLine("		, DataOra ")
            sql.AppendLine("		, Dupl = ROW_NUMBER() OVER (PARTITION BY Id_Stazione, Id_Sensore, DataOra ORDER BY Id_Stazione, Id_Sensore, DataOra) ")
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
            sql.AppendLine("    FROM MeteoNt_DatiOrari t ")
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
            sql.AppendLine("		INSERT INTO MeteoNt_DatiOrari (Id_Sensore, DataOra, Valore, Confidenza) ")
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
            sql.AppendLine("('MeteoNT_DatiOrari', @RESULT_TOTAL_ORARI, @RESULT_UPDATE_ORARI, @RESULT_INSERT_ORARI) ")
            sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]) ")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try

        Return Riepilogo
    End Function

    Public Function ScriviDatiSorgente(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.ScriviDatiSorgente"
        Dim RetValue As Boolean = True

        Dim Riepilogo As DataTable = Nothing

        Try

            Dim sql As New StringBuilder

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpDatiSorgente (Src_Id INT NOT NULL, Id_Stazione INT NOT NULL, Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, IsUpdate BIT NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiSorgente ON #TmpDatiSorgente (Id_Stazione, Id_Sensore, DataOra); ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiAggregati"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpDatiAggregati (Id_Stazione INT NOT NULL, Id_Sensore INT NOT NULL, DataOraSrc DATETIME NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, TipoSensore INT NOT NULL, FunAggreg VARCHAR(50) NOT NULL) ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiAggregati ON #TmpDatiAggregati (Id_Stazione, Id_Sensore, DataOraSrc); ")
            sql.AppendLine("CREATE INDEX IX_TmpDatiAggregati_FunAggreg ON #TmpDatiAggregati (FunAggreg) ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpUpdated (Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpUpdated ON  #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO #TmpDatiSorgente (Src_Id, Id_Stazione, Id_Sensore, DataOra, Valore, IsUpdate) ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("    T.Src_Id ")
            sql.AppendLine("    , T.Id_Stazione ")
            sql.AppendLine("    , T.Id_Sensore ")
            sql.AppendLine("    , DataOra = CASE WHEN D.UTC = 1 THEN CAST(D.DataOra AT TIME ZONE 'UTC' AT TIME ZONE T.SystemTimeZoneInfo AS DATETIME) ELSE D.DataOra END ")
            sql.AppendLine("    , D.Valore ")
            sql.AppendLine("    , 0 ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		Src_Id = src.Id ")
            sql.AppendLine("        , staz.Id AS Id_Stazione ")
            sql.AppendLine("        , sens.Id AS Id_Sensore ")
            sql.AppendLine("        , SystemTimeZoneInfo = COALESCE(staz.SystemTimeZoneInfo, 'W. Europe Standard Time') ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine()
            sql.AppendLine("		SELECT " & tblSensori.CampoId & ", ChiaveImport = " & tblSensori.ChiaveImport)
            sql.AppendLine("		FROM " & tblSensori.NomeTabella)
            sql.AppendLine()
            sql.AppendLine("	) src ")
            sql.AppendLine("    INNER JOIN MeteoNT_Sensori			sens	ON sens.ChiaveImport = src.ChiaveImport ")
            sql.AppendLine("    INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
            sql.AppendLine("    INNER JOIN MeteoNT_Stazioni			staz	ON staz.Id = sxs.Id_Stazione ")
            sql.AppendLine("    WHERE sxs.FlagReale = 1 ")
            sql.AppendLine(") T ")
            sql.AppendLine("INNER JOIN ( ")
            sql.AppendLine()
            sql.AppendLine("	SELECT Id_Sensore = " & tblDati.CampoIdSensore & ", DataOra = " & tblDati.CampoDataOra & ", Valore = " & tblDati.CampoValore & ", UTC = " & tblDati.ValoreUTC)
            sql.AppendLine("	FROM " & tblDati.NomeTabella & " WHERE " & tblDati.CampoElaborato & " = 0 ")
            sql.AppendLine()
            sql.AppendLine(") D On T.Src_Id = D.Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @RESULT_TOTAL_SORGENTE INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_UPDATE_SORGENTE INT = 0; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_SORGENTE INT = 0; ")
            sql.AppendLine("DECLARE @RESULT_TOTAL_ORARI INT = 0; ")
            sql.AppendLine("DECLARE @RESULT_UPDATE_ORARI INT = 0; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_ORARI INT = 0; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("WITH cte AS ( ")
            sql.AppendLine("    SELECT ")
            sql.AppendLine("		Src_Id ")
            sql.AppendLine("		, Id_Stazione ")
            sql.AppendLine("		, Id_Sensore ")
            sql.AppendLine("		, DataOra ")
            sql.AppendLine("		, Dupl = ROW_NUMBER() OVER (PARTITION BY Id_Stazione, Id_Sensore, DataOra ORDER BY Id_Stazione, Id_Sensore, DataOra) ")
            sql.AppendLine("     FROM #TmpDatiSorgente ")
            sql.AppendLine(") ")
            sql.AppendLine("DELETE FROM cte ")
            sql.AppendLine("WHERE Dupl > 1; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("SET @RESULT_TOTAL_SORGENTE = @RESULT_TOTAL_SORGENTE - @@ROWCOUNT; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("	BEGIN TRAN; ")
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	Aggiorno/Inserisco i dati nella tabella DatiSorgente ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	UPDATE t ")
            sql.AppendLine("	SET t.Valore = s.Valore ")
            sql.AppendLine("    OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine("    FROM MeteoNT_DatiSorgente t ")
            sql.AppendLine("    INNER JOIN #TmpDatiSorgente s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra ")
            sql.AppendLine()
            sql.AppendLine("	SET @RESULT_UPDATE_SORGENTE = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	IF (@RESULT_UPDATE_SORGENTE < @RESULT_TOTAL_SORGENTE) -- if all rows were updates then skip, else insert remaining ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine()
            sql.AppendLine("		UPDATE s ")
            sql.AppendLine("		SET s.IsUpdate = 1 ")
            sql.AppendLine("		FROM #TmpDatiSorgente s ")
            sql.AppendLine("		INNER JOIN #TmpUpdated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
            sql.AppendLine()
            sql.AppendLine("		INSERT INTO MeteoNT_DatiSorgente (Id_Sensore, DataOra, Valore) ")
            sql.AppendLine("        SELECT Id_Sensore, DataOra, Valore ")
            sql.AppendLine("        FROM #TmpDatiSorgente ")
            sql.AppendLine("        WHERE IsUpdate = 0 ")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_SORGENTE = @@ROWCOUNT ")
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
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	Produco i dati aggregati da dati originali Tabella DatiSorgente -> Tabelle DatiOrari ")
            sql.AppendLine("	Stessi sensori aggregati secondo la funzione di aggregazione propria del tipo sensore ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	INSERT INTO #TmpDatiAggregati (Id_Stazione, DataOraSrc, DataOra, Id_Sensore, Valore, TipoSensore, FunAggreg) ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		sxs.Id_Stazione ")
            sql.AppendLine("		, DataOraSrc = DataOra ")
            sql.AppendLine("		, DataOra = DATEADD(hh, DATEDIFF(hh, '19000101', DataOra), '19000101') ")
            sql.AppendLine("		, sxs.Id_Sensore ")
            sql.AppendLine("		, dati.Valore ")
            sql.AppendLine("		, Id_TipoSensore ")
            sql.AppendLine("		, FunAggreg ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT Id_Sensore, Min_DT = DATEADD(hh, DATEDIFF(hh, '19000101', MIN(DataOra)), '19000101'), Max_DT = DATEADD(hh, DATEDIFF(hh, '19000101', MAX(DataOra)) + 1, '19000101') ")
            sql.AppendLine("		FROM #TmpDatiSorgente ")
            sql.AppendLine("		GROUP BY Id_Sensore ")
            sql.AppendLine("	) app ")
            sql.AppendLine("	INNER JOIN MeteoNT_Sensori			sens	ON sens.Id = app.Id_Sensore ")
            sql.AppendLine("	INNER JOIN MeteoNT_TipiSensore		tipi	ON tipi.Id = sens.Id_TipoSensore ")
            sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
            sql.AppendLine("	INNER JOIN MeteoNT_DatiSorgente		dati	ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	WHERE sxs.FlagReale = 1 AND app.Min_DT <= dati.DataOra AND dati.DataOra < app.Max_DT ")
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	AGGREGAZIONE PER SENSORI VETTORIALI ")
            sql.AppendLine()
            sql.AppendLine("	La funzione di aggregazione di un tipo vettoriale è definita come avgvett(x) con x il tipo del sensore che identifica ")
            sql.AppendLine("	la direzione (angolo) del vettore la cui velocità (magnitudine) è il tipo stesso. ")
            sql.AppendLine("	Ad esempio per una coppia di vettori che acquisiscono la direzione e velocità del vento ")
            sql.AppendLine("	la direzione (Id Tipo = 6, ad esempio) non ha funzione di aggregazione nella tabella dei TipiSensore, ")
            sql.AppendLine("	mentre la velocità (Id Tipo = 7 ad esempio) ha come funzione di aggregazione avgvett(6) ")
            sql.AppendLine("	ad indicare che il tipo 7 sarà accoppiato al tipo 6 nei calcoli di aggregazione. ")
            sql.AppendLine()
            sql.AppendLine("	N.B. Fallisce se in uno stesso insieme ho due sensori dello stesso tipo, ad esempio due anemometri, ")
            sql.AppendLine("	che hanno la stessa funzione di aggreazione avgvett(x) ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL, Confidenza SMALLINT) ")
            sql.AppendLine("	INSERT INTO @TBL_AGGR_VET ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		DataOra ")
            sql.AppendLine("		, Id_SensoreDir ")
            sql.AppendLine("		, Dir = CASE WHEN ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001 THEN 0 ")
            sql.AppendLine("				ELSE DEGREES(ATN2(Vel_eo, Vel_ns) + (CASE WHEN ATN2(Vel_eo, Vel_ns) < PI() THEN PI() ELSE -PI() END)) END ")
            sql.AppendLine("		, Id_SensoreVel ")
            sql.AppendLine("		, Vel = SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo)) ")
            sql.AppendLine("		, Confidenza ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT ")
            sql.AppendLine("			DataOra ")
            sql.AppendLine("			, Id_SensoreDir ")
            sql.AppendLine("			, Id_SensoreVel ")
            sql.AppendLine("			, Vel_ns = -SUM(Vel_ns) / CAST(COUNT(*) AS REAL) ")
            sql.AppendLine("			, Vel_eo = -SUM(Vel_eo) / CAST(COUNT(*) AS REAL) ")
            sql.AppendLine("			, COUNT(*) AS Confidenza ")
            sql.AppendLine("		FROM ( ")
            sql.AppendLine("			SELECT ")
            sql.AppendLine("				tblDir.DataOra ")
            sql.AppendLine("				, Id_SensoreDir = tblDir.Id_Sensore ")
            sql.AppendLine("				, Id_SensoreVel = tblVel.Id_Sensore ")
            sql.AppendLine("				, Vel_ns = COS(RADIANS(tblDir.Valore)) * tblVel.Valore ")
            sql.AppendLine("				, Vel_eo = SIN(RADIANS(tblDir.Valore)) * tblVel.Valore ")
            sql.AppendLine("			FROM ( ")
            sql.AppendLine("				SELECT DISTINCT ")
            sql.AppendLine("					Id_Stazione ")
            sql.AppendLine("					, DataOraSrc ")
            sql.AppendLine("					, DataOra ")
            sql.AppendLine("					, TipoDir = CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT) ")
            sql.AppendLine("					, TipoVel = TipoSensore ")
            sql.AppendLine("				FROM #TmpDatiAggregati ")
            sql.AppendLine("				WHERE FunAggreg LIKE 'avgvett(%)' ")
            sql.AppendLine("			) t1 ")
            sql.AppendLine("			INNER JOIN #TmpDatiAggregati tblDir ON tblDir.Id_Stazione = t1.Id_Stazione AND tblDir.DataOraSrc = t1.DataOraSrc AND tblDir.TipoSensore = t1.TipoDir ")
            sql.AppendLine("			INNER JOIN #TmpDatiAggregati tblVel ON tblVel.Id_Stazione = t1.Id_Stazione AND tblVel.DataOraSrc = t1.DataOraSrc AND tblVel.TipoSensore = t1.TipoVel ")
            sql.AppendLine("		) t2 ")
            sql.AppendLine("		GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel ")
            sql.AppendLine("	) t3 ")
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	Inserisco i dati nella tabella DatiOrari ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @TBL_ORARI TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL, Confidenza SMALLINT, IsUpdate BIT NOT NULL DEFAULT (0)) ")
            sql.AppendLine("	INSERT INTO @TBL_ORARI (Id_Sensore, DataOra, Valore, Confidenza) ")
            sql.AppendLine("	SELECT Id_Sensore, DataOra, Valore, Confidenza ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = AVG(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'avg' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = SUM(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'sum' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = MAX(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'max' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = MIN(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'min' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore ")
            sql.AppendLine("			, Valore = CASE WHEN CAST( SUM( CASE WHEN Valore = 0 THEN 1 ELSE 0 END ) AS REAL) / CAST( COUNT(*) AS REAL) >= 0.5 THEN 0 ELSE 100 END ")
            sql.AppendLine("			, Confidenza = COUNT(*) ")
            sql.AppendLine("		FROM #TmpDatiAggregati WHERE FunAggreg = 'bagnatura' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore = Id_SensoreDir, Valore = Dir, Confidenza FROM @TBL_AGGR_VET ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore = Id_SensoreVel, Valore = Vel, Confidenza FROM @TBL_AGGR_VET ")
            sql.AppendLine("	) s1 ")
            sql.AppendLine()
            sql.AppendLine("	SET @RESULT_TOTAL_ORARI = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	DELETE FROM #TmpUpdated ")
            sql.AppendLine()
            sql.AppendLine("	UPDATE t ")
            sql.AppendLine("	SET t.Valore = s.Valore, t.Confidenza = s.Confidenza ")
            sql.AppendLine("	OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine("	FROM MeteoNT_DatiOrari t ")
            sql.AppendLine("	INNER JOIN @TBL_ORARI s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra ")
            sql.AppendLine()
            sql.AppendLine("	SET @RESULT_UPDATE_ORARI = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	IF (@RESULT_UPDATE_ORARI < @RESULT_TOTAL_ORARI) -- if all rows were updates then skip, else insert remaining ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine()
            sql.AppendLine("		UPDATE s ")
            sql.AppendLine("		SET s.IsUpdate = 1 ")
            sql.AppendLine("		FROM @TBL_ORARI s ")
            sql.AppendLine("		INNER JOIN #TmpUpdated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
            sql.AppendLine()
            sql.AppendLine("		INSERT INTO MeteoNT_DatiOrari (Id_Sensore, DataOra, Valore, Confidenza) ")
            sql.AppendLine("		SELECT Id_Sensore, DataOra, Valore, Confidenza ")
            sql.AppendLine("		FROM @TBL_ORARI ")
            sql.AppendLine("		WHERE IsUpdate = 0 ")
            sql.AppendLine()
            sql.AppendLine("		SET @RESULT_INSERT_ORARI = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	END; ")
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
            sql.AppendLine("	--THROW; ")
            sql.AppendLine("END CATCH; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiAggregati"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpUpdated"))
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("--Ritorno un riepilogo ")
            sql.AppendLine()
            sql.AppendLine("SELECT T.* FROM (VALUES ")
            sql.AppendLine("('MeteoNT_DatiSorgente', @RESULT_TOTAL_SORGENTE, @RESULT_UPDATE_SORGENTE, @RESULT_INSERT_SORGENTE), ")
            sql.AppendLine("('MeteoNT_DatiOrari', @RESULT_TOTAL_ORARI, @RESULT_UPDATE_ORARI, @RESULT_INSERT_ORARI) ")
            sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]) ")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try

        Return Riepilogo
    End Function

    Private Function ScriviDatiSorgente_(ByVal tabellaSource As String, ByVal sqlSource As String, ByVal flagUTC As Boolean, ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "MeteoNT.ScriviDatiSorgente"
        Dim MessaggioErrore As String = ""
        Dim sqlFrom As New StringBuilder
        Dim sql As New StringBuilder
        Dim RetValue As Boolean = True

        Try

            sqlFrom.Clear()
            sqlFrom.Append("FROM (")
            sqlFrom.Append(sqlSource)
            sqlFrom.Append(") dati ")
            sqlFrom.AppendLine("INNER JOIN MeteoNT_Sensori			nt_s	ON nt_s.ChiaveImport = dati.ChiaveImport")
            sqlFrom.AppendLine("INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = nt_s.Id ")
            sqlFrom.AppendLine("INNER JOIN MeteoNT_Stazioni			staz	ON staz.Id = sxs.Id_Stazione ")
            sqlFrom.AppendLine("WHERE sxs.FlagReale = 1")

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("BEGIN TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBLRESULT TABLE(Tabella VARCHAR(50), Operazione VARCHAR(20), Conteggio INT) ")
            sql.AppendLine("DECLARE @RiepilogoMerge TABLE(Operazione VARCHAR(20)) ")
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Leggo i dati per la tabella specifica del fornitore")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("DECLARE @SRCTBL TABLE(Id_Stazione INT, Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
            sql.AppendLine("INSERT INTO @SRCTBL ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	staz.Id AS Id_Stazione ")
            sql.AppendLine("    , nt_s.Id AS Id_Sensore ")
            If flagUTC Then
                sql.AppendLine("	, CAST(DataOraUTC AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(staz.SystemTimeZoneInfo, 'W. Europe Standard Time') AS DATETIME) AS DataOra ")
            Else
                sql.AppendLine("	, DataOra ")
            End If
            sql.AppendLine("	, Valore ")
            sql.Append(sqlFrom.ToString)
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Inserisco i dati nella tabella DatiSorgente")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("MERGE INTO MeteoNT_DatiSorgente AS t ")
            sql.AppendLine("USING ")
            sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore FROM @SRCTBL) AS s ")
            sql.AppendLine("ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
            sql.AppendLine("WHEN MATCHED THEN ")
            sql.AppendLine("	UPDATE SET Valore = s.Valore")
            sql.AppendLine("WHEN NOT MATCHED THEN ")
            sql.AppendLine("	INSERT (Id_Sensore, DataOra, Valore) ")
            sql.AppendLine("	VALUES (s.Id_Sensore, s.DataOra, s.Valore) ")
            sql.AppendLine("OUTPUT $action INTO @RiepilogoMerge; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBLRESULT ")
            sql.AppendLine("SELECT 'MeteoNT_DatiSorgente', Operazione, COUNT(*) AS Conteggio FROM @RiepilogoMerge GROUP BY Operazione ")
            sql.AppendLine()
            sql.AppendLine("DELETE FROM @RiepilogoMerge ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Aggiorno il flag elaborato per ogni sensore coinvolto")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("UPDATE " & tabellaSource)
            sql.AppendLine("SET Elaborato = 1 ")
            sql.AppendLine("WHERE Elaborato = 0 ")
            sql.AppendLine("AND Id_Sensore IN ( ")
            sql.AppendLine("SELECT DISTINCT dati.Id_Sensore ")
            sql.Append(sqlFrom.ToString)
            sql.AppendLine(")")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Per ogni sensore memorizzo le date (orarie) min/max")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("DECLARE @SENSORI_DT table(Id_Sensore INT, Min_DataOra DATETIME, Max_DataOra DATETIME) ")
            sql.AppendLine("INSERT INTO @SENSORI_DT ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id_Sensore ")
            sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', MIN(DataOra)), '19000101') AS Min_DataOra ")
            sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', MAX(DataOra)) + 1, '19000101') AS Max_DataOra ")
            sql.AppendLine("FROM @SRCTBL ")
            sql.AppendLine("GROUP BY Id_Sensore")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Produco i dati aggregati da dati originali Tabella DatiSorgente -> Tabelle DatiOrari")
            sql.AppendLine("Stessi sensori aggregati secondo la funzione di aggregazione propria del tipo sensore")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, DataOraSrc DATETIME, DataOra DATETIME, Id_Sensore INT, Valore REAL, TipoSensore INT, FunAggreg VARCHAR(50)) ")
            sql.AppendLine("INSERT INTO @TMPTABLE ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("    stazXsens.Id_Stazione ")
            sql.AppendLine("	, DataOra AS DataOraSrc ")
            sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', DataOra), '19000101') AS DataOra ")
            sql.AppendLine("	, dati.Id_Sensore ")
            sql.AppendLine("	, Valore ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, FunAggreg ")
            sql.AppendLine("FROM MeteoNT_DatiSorgente           dati ")
            sql.AppendLine("INNER JOIN MeteoNT_Sensori          sens        ON sens.Id = dati.Id_Sensore ")
            sql.AppendLine("INNER JOIN MeteoNT_TipiSensore      tipi        ON tipi.Id = sens.Id_TipoSensore ")
            sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori stazXsens   ON stazXsens.Id_Sensore = dati.Id_Sensore")
            sql.AppendLine("INNER JOIN @SENSORI_DT              sdt         ON sdt.Id_Sensore = dati.Id_Sensore ")
            sql.AppendLine("WHERE sdt.Min_DataOra <= dati.DataOra AND dati.DataOra < sdt.Max_DataOra ")
            sql.AppendLine("AND stazXsens.FlagReale = 1 ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("AGGREGAZIONE PER SENSORI VETTORIALI")
            sql.AppendLine()
            sql.AppendLine("La funzione di aggregazione di un tipo vettoriale è definita come avgvett(x) con x il tipo del sensore che identifica")
            sql.AppendLine("la direzione (angolo) del vettore la cui velocità (magnitudine) è il tipo stesso.")
            sql.AppendLine("Ad esempio per una coppia di vettori che acquisiscono la direzione e velocità del vento ")
            sql.AppendLine("la direzione (Id Tipo = 6, ad esempio) non ha funzione di aggregazione nella tabella dei TipiSensore, ")
            sql.AppendLine("mentre la velocità (Id Tipo = 7 ad esempio) ha come funzione di aggregazione avgvett(6)")
            sql.AppendLine("ad indicare che il tipo 7 sarà accoppiato al tipo 6 nei calcoli di aggregazione.")
            sql.AppendLine()
            sql.AppendLine("N.B. Fallisce se in uno stesso insieme ho due sensori dello stesso tipo, ad esempio due anemometri, ")
            sql.AppendLine("che hanno la stessa funzione di aggreazione avgvett(x)")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL, Confidenza SMALLINT) ")
            sql.AppendLine("INSERT INTO @TBL_AGGR_VET ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	DataOra ")
            sql.AppendLine("	, Id_SensoreDir ")
            sql.AppendLine("	, CASE WHEN ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001 ")
            sql.AppendLine("		THEN 0 ")
            sql.AppendLine("		ELSE DEGREES(ATN2(Vel_eo, Vel_ns) + (CASE WHEN ATN2(Vel_eo, Vel_ns) < PI() THEN PI() ELSE -PI() END)) ")
            sql.AppendLine("		END AS Dir ")
            sql.AppendLine("	, Id_SensoreVel ")
            sql.AppendLine("	, SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo)) AS Vel ")
            sql.AppendLine("	, Confidenza ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		DataOra ")
            sql.AppendLine("		, Id_SensoreDir ")
            sql.AppendLine("		, Id_SensoreVel ")
            sql.AppendLine("		, -SUM(Vel_ns) / CAST(COUNT(*) AS REAL) AS Vel_ns ")
            sql.AppendLine("		, -SUM(Vel_eo) / CAST(COUNT(*) AS REAL) AS Vel_eo ")
            sql.AppendLine("		, COUNT(*) AS Confidenza ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT ")
            sql.AppendLine("			tblDir.DataOra ")
            sql.AppendLine("			, tblDir.Id_Sensore AS Id_SensoreDir ")
            sql.AppendLine("			, tblVel.Id_Sensore AS Id_SensoreVel ")
            sql.AppendLine("			, COS(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_ns ")
            sql.AppendLine("			, SIN(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_eo ")
            sql.AppendLine("		FROM ( ")
            sql.AppendLine("			SELECT DISTINCT ")
            sql.AppendLine("				Id_Stazione ")
            sql.AppendLine("				, DataOraSrc ")
            sql.AppendLine("				, DataOra ")
            sql.AppendLine("				, CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT) AS TipoDir ")
            sql.AppendLine("				, TipoSensore AS TipoVel ")
            sql.AppendLine("			FROM @TMPTABLE ")
            sql.AppendLine("			WHERE FunAggreg LIKE 'avgvett(%)' ")
            sql.AppendLine("		) t1 ")
            sql.AppendLine("		INNER JOIN @TMPTABLE tblDir ON tblDir.Id_Stazione = t1.Id_Stazione AND tblDir.DataOraSrc = t1.DataOraSrc AND tblDir.TipoSensore = t1.TipoDir ")
            sql.AppendLine("		INNER JOIN @TMPTABLE tblVel ON tblVel.Id_Stazione = t1.Id_Stazione AND tblVel.DataOraSrc = t1.DataOraSrc AND tblVel.TipoSensore = t1.TipoVel ")
            sql.AppendLine("	) t2 ")
            sql.AppendLine("	GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel ")
            sql.AppendLine(") t3 ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("/**************************************************************************************************")
            sql.AppendLine("Inserisco i dati nella tabella DatiOrari")
            sql.AppendLine("**************************************************************************************************/")
            sql.AppendLine("MERGE INTO MeteoNT_DatiOrari AS t ")
            sql.AppendLine("USING ")
            sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore, Confidenza ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore, COUNT(*) AS Confidenza ")
            sql.AppendLine("		FROM @TMPTABLE ")
            sql.AppendLine("		WHERE FunAggreg = 'avg' ")
            sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine("		UNION ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore, COUNT(*) AS Confidenza ")
            sql.AppendLine("		FROM @TMPTABLE ")
            sql.AppendLine("		WHERE FunAggreg = 'sum' ")
            sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine("		UNION ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore, COUNT(*) AS Confidenza ")
            sql.AppendLine("		FROM @TMPTABLE ")
            sql.AppendLine("		WHERE FunAggreg = 'max' ")
            sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine("		UNION ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_Sensore, CASE WHEN CAST( SUM( CASE WHEN Valore = 0 THEN 1 ELSE 0 END ) AS REAL) / CAST( COUNT(*) AS REAL) >= 0.5 THEN 0 ELSE 100 END AS Valore, COUNT(*) AS Confidenza ")
            sql.AppendLine("		FROM @TMPTABLE ")
            sql.AppendLine("		WHERE FunAggreg = 'bagnatura' ")
            sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine("		UNION ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_SensoreDir AS Id_Sensore, Dir AS Valore, Confidenza ")
            sql.AppendLine("		FROM @TBL_AGGR_VET ")
            sql.AppendLine()
            sql.AppendLine("		UNION ")
            sql.AppendLine()
            sql.AppendLine("		SELECT DataOra, Id_SensoreVel AS Id_Sensore, Vel AS Valore, Confidenza ")
            sql.AppendLine("		FROM @TBL_AGGR_VET ")
            sql.AppendLine("	) s1 ")
            sql.AppendLine(") AS s ")
            sql.AppendLine("ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
            sql.AppendLine("WHEN MATCHED THEN ")
            sql.AppendLine("	UPDATE SET Valore = s.Valore, Confidenza = s.Confidenza ")
            sql.AppendLine("WHEN NOT MATCHED THEN ")
            sql.AppendLine("	INSERT (Id_Sensore, DataOra, Valore, Confidenza) ")
            sql.AppendLine("	VALUES (s.Id_Sensore, s.DataOra, s.Valore, s.Confidenza) ")
            sql.AppendLine("OUTPUT $action INTO @RiepilogoMerge; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBLRESULT ")
            sql.AppendLine("SELECT 'MeteoNT_DatiOrari', Operazione, COUNT(*) AS Conteggio FROM @RiepilogoMerge GROUP BY Operazione ")
            sql.AppendLine()
            sql.AppendLine("--Ritorno un riepilogo")
            sql.AppendLine("SELECT * FROM @TBLRESULT ")
            sql.AppendLine()
            sql.AppendLine("COMMIT TRANSACTION ")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            'RIEPILOGO RITORNATO...
            'MeteoNT_DatiSorgente	INSERT	xxx
            'MeteoNT_DatiSorgente	UPDATE	yyy
            'MeteoNT_DatiOrari	    INSERT	zzz
            'MeteoNT_DatiOrari	    UPDATE	vvv

        Catch ex As Exception

            RetValue = False
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return RetValue

    End Function

    Public Function ScriviDatiSorgente_WiNet_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOraUTC ")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'WINET-' + CAST(s.Id_Rete AS VARCHAR(10)) + '-' + CAST(s.Id_Nodo AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
        sql.AppendLine("FROM WiNet2_Dati			d ")
        sql.AppendLine("INNER JOIN WiNet2_Sensori	s	ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("WiNet2_Dati", sql.ToString, True, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_AS_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOraUTC ")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'AS-' + CAST(s.Id_Sistema AS VARCHAR(10)) + '-' + CAST(s.Id_Unita AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
        sql.AppendLine("FROM AS_Dati			d ")
        sql.AppendLine("INNER JOIN AS_Sensori	s	ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("AS_Dati", sql.ToString, True, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_NetSens_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOraUTC ")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'NETSENS-' + CAST(s.Id_Stazione AS VARCHAR(10)) + '-' + CAST(s.Id_Unita AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
        sql.AppendLine("FROM NetSens_Dati		    d ")
        sql.AppendLine("INNER JOIN NetSens_Sensori	s	ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("NetSens_Dati", sql.ToString, True, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_Pessl_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOra")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'PESSL-' + TRIM(Id_Station) + '-' + CAST(Ch AS VARCHAR(20)) + '-' + Mac + '-' + Serial + '-' + CAST(Code AS VARCHAR(20)) + '-' + Aggr AS ChiaveImport ")
        sql.AppendLine("FROM Pessl_Dati		        d ")
        sql.AppendLine("INNER JOIN Pessl_Sensori	s	ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("Pessl_Dati", sql.ToString, False, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_GreenPlanet_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOra")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'GP-' + CAST(Id_Stazione AS VARCHAR(10)) + '-' + TRIM(Sensore) AS ChiaveImport ")
        sql.AppendLine("FROM GreenPlanet_Dati          d ")
        sql.AppendLine("INNER JOIN GreenPlanet_Sensori s ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("GreenPlanet_Dati", sql.ToString, False, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_A2A_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOraUTC ")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'A2A-' + TRIM(Id_Stazione) + '-' + TRIM(MeasureName) AS ChiaveImport ")
        sql.AppendLine("FROM A2A_Dati d ")
        sql.AppendLine("INNER JOIN A2A_Sensori s ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("A2A_Dati", sql.ToString, True, ObjParametri_Server)
    End Function

    Public Function ScriviDatiSorgente_DigiFarm_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim sql As New StringBuilder
        sql.Clear()
        sql.AppendLine("SELECT ")
        sql.AppendLine("	d.Id_Sensore ")
        sql.AppendLine("	, DataOra ")
        sql.AppendLine("	, Valore ")
        sql.AppendLine("	, 'DF-' + TRIM(Id_Stazione) + '-' + TRIM(SensorId) AS ChiaveImport ")
        sql.AppendLine("FROM DigiFarm_Dati d ")
        sql.AppendLine("INNER JOIN DigiFarm_Sensori s ON s.Id = d.Id_Sensore ")
        sql.AppendLine("WHERE d.Elaborato = 0 ")

        Return ScriviDatiSorgente_("DigiFarm_Dati", sql.ToString, False, ObjParametri_Server)
    End Function


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



    Public Function LeggiDati(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer, DataInizio As DateTime, DataFine As DateTime, granularita As enum_GranularitaDati, SogliaTermica As Decimal, SogliaFabbisognoFreddo As Decimal, ObjParametri_Server As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "MeteoNT.LeggiDati"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @STAZIONE INT = " & CStr(IdStazione))
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
            If granularita <> enum_GranularitaDati.Sorgente Then
                sql.AppendLine("INSERT INTO @TBL_TMPDERIVATI VALUES ")
                sql.AppendLine("(1, 'min', ' [min]', 1, NULL, '{""serie"": ""rangeArea"", ""fieldProp"": ""fromField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}', NULL, 0, 0), ")
                sql.AppendLine("(1, 'max', ' [max]', 2, NULL, '{""serie"": ""rangeArea"", ""fieldProp"": ""toField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}', NULL, 0, 0), ")
                sql.AppendLine("(1, 't_sum', ' [Somma termica]', 3, 'Somma termica', '{""serie"": ""line"", ""colore"": ""rgb(128, 0, 128)"", ""gruppo"": 9999}', NULL, 0, 1), ")
                sql.AppendLine("(1, 't_cnt_freddo', ' [Cumulo fabbisogno freddo]', 4, 'Cumulo fabbisogno freddo', '{""serie"": ""line"", ""colore"": ""rgb(0, 206, 209)"", ""gruppo"": 9998}', 'Ore', 1, 0) ")
            End If
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_SXS TABLE(Id_Stazione INT, Id_Sensore INT, Id_TipoSensore INT, Ordine INT, OutputConfig VARCHAR(MAX), Etichetta VARCHAR(MAX)) ")

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sql.AppendLine("INSERT INTO @TBL_SXS VALUES ")
                    sql.AppendLine("(@STAZIONE, 1, 1, 1, NULL, 'Temperatura aria'), ")
                    sql.AppendLine("(@STAZIONE, 2, 3, 2, NULL, 'Umidità relativa'), ")
                    sql.AppendLine("(@STAZIONE, 3, 4, 4, NULL, 'Bagnatura fogliare'), ")
                    sql.AppendLine("(@STAZIONE, 4, 2, 3, NULL, 'Pioggia') ")
                    sql.AppendLine()
                    sql.AppendLine("DECLARE @ID_SENSORE INTEGER = (SELECT MAX(Id_Sensore) FROM @TBL_SXS) ")

                Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche

                    sql.AppendLine("INSERT INTO @TBL_SXS ")
                    sql.AppendLine("SELECT Id_Stazione, Id_Sensore, Id_TipoSensore, Ordine, OutputConfig, COALESCE(sxs.Etichetta, s.Etichetta) ")
                    sql.AppendLine("FROM MeteoNT_StazioniXSensori sxs WITH (NOLOCK)")
                    sql.AppendLine("INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) ON s.Id = sxs.Id_Sensore ")
                    sql.AppendLine("WHERE Id_Stazione = @STAZIONE")
                    sql.AppendLine()
                    sql.AppendLine("DECLARE @ID_SENSORE INTEGER = (SELECT MAX(Id) FROM MeteoNT_Sensori WITH (NOLOCK)) ")

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
            sql.AppendLine("	SELECT Id, Misura, UM, FunAggreg, DefaultOutputConfig FROM MeteoNT_TipiSensore WITH (NOLOCK) WHERE Id = 0 ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Id, Misura = COALESCE(Misura, Tipo),  UM, FunAggreg, DefaultOutputConfig FROM MeteoNT_TipiSensore WITH (NOLOCK) WHERE Id <> 0 ")
            sql.AppendLine(") tipi ON tipi.Id = sxs.Id_TipoSensore ")
            sql.AppendLine("WHERE sxs.Id_Stazione = @STAZIONE ")
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
            sql.AppendLine("INNER JOIN MeteoNT_TipiSensore tipi WITH (NOLOCK) ON tipi.Id = conv.Id_TipoDst ")
            sql.AppendLine()
            sql.AppendLine()

            Dim tblDati As String = "MeteoNT_DatiOrari"
            Dim whereDati As String = ""

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    Dim Tabella_RER As String = "Dati_Meteo_SHH"
                    Dim Campo_ID As String = "ID_Stazione"

                    If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                        Tabella_RER = "Dati_Meteo_QHH"
                        Campo_ID = "ID_Quadrante"
                    End If

                    sql.AppendLine("; WITH SRC_CTE AS ( ")
                    sql.AppendLine("    SELECT Tempo, Temp_Media, UmiditaRelativa, Bagnatura, Precipitazione FROM " & Tabella_RER & " WHERE " & Campo_ID & " = @STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE ")
                    sql.AppendLine("), ")
                    sql.AppendLine("DATI_RER AS ( ")
                    sql.AppendLine("	SELECT Id_Sensore = 1, DataOra = Tempo, Valore = Temp_Media FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 2, DataOra = Tempo, Valore = UmiditaRelativa FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 3, DataOra = Tempo, Valore = Bagnatura FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 4, DataOra = Tempo, Valore = Precipitazione FROM SRC_CTE ")
                    sql.AppendLine(") ")

                    tblDati = "DATI_RER"

                Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche

                    If TipoSorgente <> enum_Meteo_Tiposorgente.Pubbliche AndAlso granularita = enum_GranularitaDati.Sorgente Then

                        tblDati = "MeteoNT_DatiSorgente"
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
            sql.AppendLine("	FROM " & tblDati & " dati WITH (NOLOCK) ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI sens ON sens.Id_Src = dati.Id_Sensore ")
            sql.AppendLine("	" & whereDati)
            sql.AppendLine(") D ")
            sql.AppendLine("INNER JOIN @TBL_SENSORI sens ON sens.Id = D.Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine()
            sql.Append(CreateTableDati("DatiMeteo"))
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
            sql.AppendLine("	INSERT INTO #DatiMeteo ")
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
            sql.AppendLine("	INSERT INTO #DatiMeteo ")
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
            sql.AppendLine("DELETE FROM @TBL_SENSORI WHERE Id NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) ")
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
            sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
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
            sql.AppendLine("			INNER JOIN MeteoNT_StazioniXSensori sxs1 WITH (NOLOCK) ON sxs1.Id_Sensore = TDir.Id_Src AND sxs1.FlagReale = 1 ")
            sql.AppendLine("			INNER JOIN MeteoNT_StazioniXSensori sxs2 WITH (NOLOCK) ON sxs2.Id_Sensore = TVel.Id_Src AND sxs2.FlagReale = 1 AND sxs1.Id_Stazione = sxs2.Id_Stazione ")
            sql.AppendLine("		) TT2 ")
            sql.AppendLine("		INNER JOIN Dati_CTE ON Dati_CTE.Id_Sensore = TT2.Id_SensoreDir ")
            sql.AppendLine("	) T4 ON T4.Id_Sensore = T1.Id_Sensore ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI S ON S.Id = T1.Id_Sensore ")
            sql.AppendLine("	WHERE T1.FunAggreg <> '' ")
            sql.AppendLine()
            sql.AppendLine("END ")
            sql.AppendLine()
            sql.Append(DropTableDati("DatiMeteo"))

            Dim DS As New DataSet

            EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine, DS, "DatiMeteo_NT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return Nothing
    End Function






    Public Function LeggiDati_V2(params As ParametriMeteo, ObjParametri_Server As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "MeteoNT.LeggiDati_V2"

        Try
            Dim id_stazione As String = "NULL"
            Dim id_qrer As String = "NULL"
            Dim id_srer As String = "NULL"

            Select Case params.TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER
                    id_srer = $"{params.Id_Stazione}"

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti
                    id_qrer = $"{params.Id_Stazione}"

                Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche
                    id_stazione = $"{params.Id_Stazione}"

            End Select

            Dim sql As String

            sql = $"SET NOCOUNT ON 

DECLARE @ID_STAZIONE INT = {id_stazione}
DECLARE @ID_QRER INT = {id_qrer};
DECLARE @ID_SRER INT = {id_srer};
DECLARE @STARTPERIOD DATETIME = '{params.StartPeriod:yyyy-MM-ddTHH:mm:ss}';
DECLARE @ENDPERIOD DATETIME = '{params.EndPeriod:yyyy-MM-ddTHH:mm:ss}';
DECLARE @GRANULARITA_DATI TINYINT = {params.GranularitaDati:D}; --0: Dati SORGENTE, 1: Dati ORARI, 2: Aggregazione GIORNALIERA
DECLARE @SOGLIA_TERMICA REAL = {params.SogliaTermica.ToString(CultureInfo.InvariantCulture)};
DECLARE @SOGLIA_FABBISOGNO_FREDDO REAL = {params.SogliaFabbisognoFreddo.ToString(CultureInfo.InvariantCulture)};
DECLARE @WANT_PUBLIC BIT = {If(params.WantPublic, "1", "0")};
DECLARE @WANT_FORECAST BIT = {If(params.WantForecast, "1", "0")};


DECLARE @LAT FLOAT = NULL;
DECLARE @LNG FLOAT = NULL;
DECLARE @PUBLIC_FLAG BIT = 0;
DECLARE @TIMEZONE VARCHAR(MAX) = NULL; 
DECLARE @ID_PUBLIC INTEGER = 0;
DECLARE @ID_FORECAST INTEGER = 0;


IF (@ID_STAZIONE IS NOT NULL)
BEGIN

	SELECT @LAT = COALESCE(@LAT, Lat_Dec), @LNG = COALESCE(@LNG, Lng_Dec), @PUBLIC_FLAG = IIF(Categoria = 1, 1, 0), @TIMEZONE = SystemTimeZoneInfo
	FROM MeteoNT_Stazioni WITH (NOLOCK)
	WHERE Id = @ID_STAZIONE

END
ELSE
BEGIN

	SELECT TOP 1 @LAT = COALESCE(@LAT, CAST(REPLACE(REPLACE(sLat, 'E', ''), ',', '.') AS FLOAT)), @LNG = COALESCE(@LNG, CAST(REPLACE(REPLACE(sLng, 'N', ''), ',', '.') AS FLOAT))
	FROM (
		SELECT sLat = X_LON_GC, sLng = Y_LAT_GC
		FROM TB_Quadranti WITH (NOLOCK)
		WHERE ID_Quadrante = @ID_QRER
		UNION ALL 
		SELECT sLat = X_LON_GC, sLng = Y_LAT_GC
		FROM TB_QuadrantixStazioni x WITH (NOLOCK)
		INNER JOIN TB_Quadranti q WITH (NOLOCK) ON q.ID_Quadrante = x.ID_Quadrante
		WHERE x.ID_Stazione = @ID_SRER
	) T

END


IF (@LAT IS NOT NULL AND @LNG IS NOT NULL AND @GRANULARITA_DATI > 0)
BEGIN

	DECLARE @REQUEST_POINT GEOGRAPHY = GEOGRAPHY::Point(@LAT, @LNG, 4326)
	
	IF (@WANT_PUBLIC = 1 AND @PUBLIC_FLAG = 0)
	BEGIN
		SELECT @ID_PUBLIC = Id, @TIMEZONE = COALESCE(@TIMEZONE, SystemTimeZoneInfo)
		FROM (
		    SELECT TOP (1) Id, GeoEntity, Distanza = GeoEntity.STDistance(@REQUEST_POINT), SystemTimeZoneInfo
			FROM MeteoNT_Stazioni WITH (NOLOCK)
			WHERE Categoria = 1 
			ORDER BY Distanza
		) T WHERE Distanza < 50000
	END

	IF (@WANT_FORECAST = 1)
	BEGIN
	
		SELECT @ID_FORECAST = Id
		FROM (
			SELECT TOP (1) Id, Cell, Distanza = Cell.STDistance(@REQUEST_POINT)
			FROM Hypermeteo_HFS_ITA_4KM WITH (NOLOCK)
			ORDER BY Distanza
		) T WHERE Distanza < 50000
	END

END


SET @TIMEZONE = COALESCE(@TIMEZONE, 'W. Europe Standard Time');


--y = ((x + xOffset) * multiplicand / denominator) + yOffset 

DECLARE @TBL_CONV TABLE(Id_TipoSrc INT, Id_TipoDst INT, xOffs REAL, Mult REAL, Denom REAL, yOffs REAL) 
INSERT INTO @TBL_CONV VALUES 
(9, 10, 0.0, 3600.0, 1000.0, 0.0),	--Velocità vento:	m/s -> Km/h (vettoriale) 
(12, 13, 0.0, 3600.0, 1000.0, 0.0),	--Velocità raffica:	m/s -> Km/h (vettoriale) 
(23, 24, 0.0, 3600.0, 1000.0, 0.0),	--Velocità vento:	m/s -> Km/h (scalare) 
(25, 26, 0.0, 3600.0, 1000.0, 0.0),	--Velocità raffica:	m/s -> Km/h (scalare) 
(33, 32, -815.0, 1, 994.0, 0.0)		--Conducibilità elettrica: VIC -> dS/m 


DECLARE @SENSORIXSTAZIONE TABLE(Id_Sensore INT, OutputConfig VARCHAR(MAX), Ordine INT, Etichetta VARCHAR(MAX)) 

DECLARE @SENSORS_REQUEST TABLE(Id_Sensore INT, Id_TipoSensore INT, conv_xOffs REAL, conv_Mult REAL, conv_Denom REAL, conv_yOffs REAL)
DECLARE @SENSORS_PUBLIC TABLE(Id_SensoreSrc INT, Id_SensoreDst INT, Id_TipoSensore INT, conv_xOffs REAL, conv_Mult REAL, conv_Denom REAL, conv_yOffs REAL)
DECLARE @SENSORS_FORECAST TABLE(Id_SensoreSrc INT, Id_sensoreDst INT, Id_TipoSensore INT, conv_xOffs REAL, conv_Mult REAL, conv_Denom REAL, conv_yOffs REAL)

-- Ambito = 0: Dati stazione, Ambito = 1: Dati pubblici, Ambito = 2: Dati previsionali
DECLARE @METEO_TMP TABLE (DataOra DATETIME, Id_Sensore INT, Valore REAL, Ambito TINYINT)


IF (@ID_STAZIONE IS NOT NULL)
BEGIN

	INSERT INTO @SENSORIXSTAZIONE
	SELECT Id_Sensore, OutputConfig, Ordine, Etichetta FROM MeteoNT_StazioniXSensori WITH (NOLOCK) WHERE Id_Stazione = @ID_STAZIONE

	--*************************************************************************************************
	-- DEBUG ******************************************************************************************
	--*************************************************************************************************
	--INSERT INTO @SENSORIXSTAZIONE VALUES 
	--(2484807, NULL, 11, 'Air temperature, high precision'),
	--(2482369, NULL, 12, 'U-sonic wind dir'),
	--(2482370, NULL, 13, 'U-sonic wind speed')
	--*************************************************************************************************
	--*************************************************************************************************
	--*************************************************************************************************

	INSERT INTO @SENSORS_REQUEST
	SELECT 
		Id_Sensore, Id_TipoSensore = COALESCE(c.Id_TipoDst, s.Id_TipoSensore)
		, conv_xOffs = COALESCE(xOffs, 0), conv_Mult = COALESCE(Mult, 1), conv_Denom = COALESCE(Denom, 1), conv_yOffs = COALESCE(yOffs, 0)
	FROM @SENSORIXSTAZIONE x 
	INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) ON s.Id = x.Id_Sensore
	LEFT JOIN @TBL_CONV c ON c.Id_TipoSrc = s.Id_TipoSensore

	IF (@GRANULARITA_DATI > 0 OR @PUBLIC_FLAG = 1)
	BEGIN

		INSERT INTO @METEO_TMP
		SELECT DataOra, S.Id_Sensore, Valore = ((Valore + conv_xOffs) * conv_Mult / conv_Denom) + conv_yOffs, 0
		FROM MeteoNT_DatiOrari d WITH (NOLOCK) 
		INNER JOIN @SENSORS_REQUEST s ON s.Id_Sensore = d.Id_Sensore 
		WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD 

	END
	ELSE
	BEGIN

		INSERT INTO @METEO_TMP
		SELECT DataOra, S.Id_Sensore, Valore = ((Valore + conv_xOffs) * conv_Mult / conv_Denom) + conv_yOffs, 0
		FROM MeteoNT_DatiSorgente d WITH (NOLOCK) 
		INNER JOIN @SENSORS_REQUEST s ON s.Id_Sensore = d.Id_Sensore 
		WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD 

	END
END
ELSE
BEGIN

	INSERT INTO @SENSORIXSTAZIONE VALUES
	(1, NULL, 1, 'Temperatura aria'), 
	(2, NULL, 2, 'Umidità relativa'), 
	(3, NULL, 3, 'Bagnatura fogliare'), 
	(4, NULL, 4, 'Pioggia') 
	
	DECLARE @RER_SENS TABLE (Id INT, Id_TipoSensore INT)
	INSERT INTO @RER_SENS VALUES (1, 1), (2, 3), (3, 42), (4, 2)
	
	INSERT INTO @SENSORS_REQUEST
	SELECT 
		Id_Sensore, Id_TipoSensore = COALESCE(c.Id_TipoDst, s.Id_TipoSensore)
		, conv_xOffs = COALESCE(xOffs, 0), conv_Mult = COALESCE(Mult, 1), conv_Denom = COALESCE(Denom, 1), conv_yOffs = COALESCE(yOffs, 0)
	FROM @SENSORIXSTAZIONE x 
	INNER JOIN @RER_SENS s ON s.Id = x.Id_Sensore
	LEFT JOIN @TBL_CONV c ON c.Id_TipoSrc = s.Id_TipoSensore
	
	; WITH SRC_CTE AS ( 
		SELECT Tempo, Temp_Media, UmiditaRelativa, Bagnatura, Precipitazione FROM Dati_Meteo_QHH WHERE ID_Quadrante = @ID_QRER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD
		UNION ALL
		SELECT Tempo, Temp_Media, UmiditaRelativa, Bagnatura, Precipitazione FROM Dati_Meteo_SHH WHERE ID_Stazione = @ID_SRER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD
	), 
	DATI_RER AS ( 
		SELECT Id_Sensore = 1, DataOra = Tempo, Valore = Temp_Media FROM SRC_CTE 
		UNION 
		SELECT Id_Sensore = 2, DataOra = Tempo, Valore = UmiditaRelativa FROM SRC_CTE 
		UNION 
		SELECT Id_Sensore = 3, DataOra = Tempo, Valore = Bagnatura FROM SRC_CTE 
		UNION 
		SELECT Id_Sensore = 4, DataOra = Tempo, Valore = Precipitazione FROM SRC_CTE 
	) 
	
	INSERT INTO @METEO_TMP
	SELECT DataOra, S.Id_Sensore, Valore = ((Valore + conv_xOffs) * conv_Mult / conv_Denom) + conv_yOffs, 0
	FROM DATI_RER d 
	INNER JOIN @SENSORS_REQUEST s ON s.Id_Sensore = d.Id_Sensore 

END



--Eliminare i sensori che non hanno dati nel periodo specificato
DELETE S
FROM @SENSORS_REQUEST S
LEFT JOIN (SELECT DISTINCT Id_Sensore FROM @METEO_TMP) T ON T.Id_Sensore = S.Id_Sensore
WHERE T.Id_Sensore IS NULL


INSERT INTO @SENSORS_PUBLIC
SELECT T.Id_Sensore, r.Id_Sensore, T.Id_TipoSensore, T.conv_xOffs, T.conv_Mult, T.conv_Denom, T.conv_yOffs FROM 
(
	SELECT 
		Id_Sensore, Id_TipoSensore = COALESCE(c.Id_TipoDst, s.Id_TipoSensore)
		, conv_xOffs = COALESCE(xOffs, 0), conv_Mult = COALESCE(Mult, 1), conv_Denom = COALESCE(Denom, 1), conv_yOffs = COALESCE(yOffs, 0)
	FROM MeteoNT_StazioniXSensori x WITH (NOLOCK)
	INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) ON s.Id = x.Id_Sensore
	LEFT JOIN @TBL_CONV c ON c.Id_TipoSrc = s.Id_TipoSensore
	WHERE Id_Stazione = @ID_PUBLIC
) T 
INNER JOIN @SENSORS_REQUEST r ON r.Id_TipoSensore = T.Id_TipoSensore


INSERT INTO @SENSORS_FORECAST
SELECT T.Id_Sensore, r.Id_Sensore, T.Id_TipoSensore, T.conv_xOffs, T.conv_Mult, T.conv_Denom, T.conv_yOffs FROM 
(
	SELECT 
		Id_Sensore = s.Id, Id_TipoSensore = COALESCE(c.Id_TipoDst, t.IdSource)
		, conv_xOffs = COALESCE(xOffs, 0), conv_Mult = COALESCE(Mult, 1), conv_Denom = COALESCE(Denom, 1), conv_yOffs = COALESCE(yOffs, 0)
	FROM Hypermeteo_FORECAST_METEO_Sensori s WITH (NOLOCK)
	INNER JOIN Hypermeteo_FORECAST_METEO_TipiSensore t WITH (NOLOCK) ON t.Id = s.Id_TipoSensore
	LEFT JOIN @TBL_CONV c ON c.Id_TipoSrc = t.IdSource
	WHERE Id_Stazione = @ID_FORECAST
) T 
INNER JOIN @SENSORS_REQUEST r ON r.Id_TipoSensore = T.Id_TipoSensore



INSERT INTO @METEO_TMP
SELECT DataOra, p.Id_SensoreDst, Valore = ((Valore + p.conv_xOffs) * p.conv_Mult / p.conv_Denom) + p.conv_yOffs, 1
FROM MeteoNT_DatiOrari d WITH (NOLOCK) 
INNER JOIN @SENSORS_PUBLIC p ON p.Id_SensoreSrc = d.Id_Sensore
WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD 


INSERT INTO @METEO_TMP
SELECT DataOra, Id_Sensore, Valore, 2
FROM (
	SELECT
		DataOra = CAST(DataOraUTC AT TIME ZONE 'UTC' AT TIME ZONE @TIMEZONE AS DATETIME)
		, Id_Sensore = f.Id_sensoreDst
		, f.Id_TipoSensore
		, Valore = ((Valore + f.conv_xOffs) * f.conv_Mult / f.conv_Denom) + f.conv_yOffs 
	FROM Hypermeteo_FORECAST_METEO_Dati d WITH (NOLOCK) 
	INNER JOIN @SENSORS_FORECAST f ON f.Id_SensoreSrc = D.Id_Sensore 
) T
WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD 



DECLARE @TBL_TMPDERIVATI TABLE(Id_TipoSensoreSrc INT, FunDeriv VARCHAR(20), Descrizione VARCHAR(MAX), Ordine INT, Misura VARCHAR(MAX), OutputConfig VARCHAR(MAX), UM VARCHAR(10), HourlyOutput BIT, DailyOutput BIT) 
INSERT INTO @TBL_TMPDERIVATI VALUES 
(1, 'min', ' [min]', 1, NULL, '{{""serie"": ""rangeArea"", ""fieldProp"": ""fromField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}}', NULL, 0, 1), 
(1, 'max', ' [max]', 2, NULL, '{{""serie"": ""rangeArea"", ""fieldProp"": ""toField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}}', NULL, 0, 1), 
(1, 't_sum', ' [Somma termica]', 3, 'Somma termica', '{{""serie"": ""line"", ""colore"": ""rgb(128, 0, 128)"", ""gruppo"": 9999}}', NULL, 1, 1), 
(1, 't_cnt_freddo', ' [Cumulo fabbisogno freddo]', 4, 'Cumulo fabbisogno freddo', '{{""serie"": ""line"", ""colore"": ""rgb(0, 206, 209)"", ""gruppo"": 9998}}', 'Ore', 0, 1) 


DECLARE @SENSORS_OUT TABLE(Id_Sensore INT, Id_TipoSensoreSrc INT, FunAggreg VARCHAR(20), Etichetta VARCHAR(MAX), DescrAggr VARCHAR(50), Ordine INT, Misura VARCHAR(50), OutputConfig VARCHAR(MAX), UM VARCHAR(10), Sensore VARCHAR(50), Derived BIT) 

INSERT INTO @SENSORS_OUT
SELECT 
	r.Id_Sensore
	, Id_TipoSensoreSrc = r.Id_TipoSensore
	, FunAggreg
	, Etichetta = COALESCE(x.Etichetta, s.Etichetta)
	, ''
	, Ordine = Ordine * 10
	, Misura = COALESCE(Misura, Tipo)
	, OutputConfig = COALESCE(x.OutputConfig, t.DefaultOutputConfig, '') 
	, UM
	, 'S_' + CAST(r.Id_Sensore AS VARCHAR(10))
	, 0
FROM @SENSORIXSTAZIONE x 
INNER JOIN @SENSORS_REQUEST r ON r.Id_Sensore = x.Id_Sensore
INNER JOIN MeteoNT_TipiSensore t ON t.Id = r.Id_TipoSensore
INNER JOIN MeteoNT_Sensori s ON s.Id = r.Id_Sensore


INSERT INTO @SENSORS_OUT
SELECT 
	o.Id_Sensore
	, d.Id_TipoSensoreSrc 
	, FunDeriv
	, Etichetta
	, Descrizione 
	, Ordine = o.Ordine + d.Ordine
	, Misura = COALESCE(d.Misura, o.Misura)
	, OutputConfig = COALESCE(d.OutputConfig, '') 
	, COALESCE(d.UM, o.UM)
	, 'SD_'+ UPPER(FunDeriv) + '_' + CAST(o.Id_Sensore AS VARCHAR(10))
	, 1
FROM @SENSORS_OUT o
INNER JOIN @TBL_TMPDERIVATI d ON d.Id_TipoSensoreSrc = o.Id_TipoSensoreSrc 
WHERE (d.DailyOutput = 1 AND @GRANULARITA_DATI = 2) OR (d.HourlyOutput = 1 AND @GRANULARITA_DATI = 1)


DECLARE @WIND_MATCH TABLE (Id_SensoreVel INT, Id_SensoreDir INT)
INSERT INTO @WIND_MATCH
SELECT DISTINCT Id_SensoreVel, Id_SensoreDir = TDir.Id_Sensore FROM ( 
	SELECT Id_SensoreVel = Id_Sensore, TipoDir = CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT)
	FROM @SENSORS_OUT
	WHERE FunAggreg LIKE 'avgvett(%)' 
) T 
INNER JOIN @SENSORS_OUT TDir ON TDir.Id_TipoSensoreSrc = T.TipoDir
INNER JOIN MeteoNT_StazioniXSensori x1 WITH (NOLOCK) ON x1.Id_Sensore = TDir.Id_Sensore AND x1.FlagReale = 1 
INNER JOIN MeteoNT_StazioniXSensori x2 WITH (NOLOCK) ON x2.Id_Sensore = T.Id_SensoreVel AND x2.FlagReale = 1 AND x1.Id_Stazione = x2.Id_Stazione 


DECLARE @METEO_DATA TABLE (DataOraSrc DATETIME, DataOra DATETIME, Id_Sensore INT, Valore REAL)
INSERT INTO @METEO_DATA
SELECT
	DataOraSrc = DataOra 
	, DataOra = CASE WHEN @GRANULARITA_DATI = 2 THEN DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)) ELSE DataOra END 
	, Id_Sensore
	, Valore
FROM (
	SELECT DataOra, Id_Sensore, Valore, Seq = ROW_NUMBER() OVER (PARTITION BY DataOra, Id_Sensore ORDER BY DataOra, Id_Sensore, Ambito)
	FROM @METEO_TMP
) T WHERE Seq = 1


BEGIN TRY
	DROP TABLE #Meteo
END TRY
BEGIN CATCH
END CATCH

CREATE TABLE #Meteo (DataOra DATETIME, Sensore VARCHAR(50), Valore REAL) 


IF @GRANULARITA_DATI = 2
BEGIN 

	DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL) 
	INSERT INTO @TBL_AGGR_VET 
	SELECT
		DataOra 
		, Id_SensoreDir 
		, Dir = IIF(ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001, 0, DEGREES(ATN2(Vel_eo, Vel_ns) + (IIF(ATN2(Vel_eo, Vel_ns) < PI(), PI(), -PI())))) 
		, Id_SensoreVel 
		, Vel = SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo))
	FROM (
		SELECT  
			DataOra 
			, Id_SensoreDir 
			, Id_SensoreVel 
			, Vel_ns = -SUM(Vel_ns) / CAST(COUNT(*) AS REAL)
			, Vel_eo = -SUM(Vel_eo) / CAST(COUNT(*) AS REAL)
		FROM ( 
			SELECT 
				DIR.DataOra
				, Id_SensoreDir
				, Id_SensoreVel
				, Vel_ns = COS(RADIANS(DIR.Valore)) * VEL.Valore 
				, Vel_eo = SIN(RADIANS(DIR.Valore)) * VEL.Valore
			FROM @WIND_MATCH W
			INNER JOIN @METEO_DATA DIR ON DIR.Id_Sensore = W.Id_SensoreDir
			INNER JOIN @METEO_DATA VEL ON VEL.Id_Sensore = W.Id_SensoreVel AND VEL.DataOraSrc = DIR.DataOraSrc
		) T
		GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel 
	) T


	; WITH DATA_CTE AS (
		SELECT DataOra, Sensore, Valore, FunAggreg
		FROM @METEO_DATA D 
		INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = D.Id_Sensore
	)

	INSERT INTO #Meteo 

	SELECT DataOra, Sensore, Valore = AVG(Valore) FROM DATA_CTE WHERE FunAggreg = 'avg' GROUP BY DataOra, Sensore 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = MIN(Valore) FROM DATA_CTE WHERE FunAggreg = 'min' GROUP BY DataOra, Sensore 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = MAX(Valore) FROM DATA_CTE WHERE FunAggreg = 'max' GROUP BY DataOra, Sensore 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = SUM(Valore) FROM DATA_CTE WHERE FunAggreg = 'sum' GROUP BY DataOra, Sensore 
	UNION --Ore di bagnatura 
	SELECT DataOra, Sensore, Valore = SUM(IIF(Valore > 0, 1, 0)) FROM DATA_CTE WHERE FunAggreg = 'bagnatura' GROUP BY DataOra, Sensore 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = SUM(IIF(0 > Valore, 0, Valore)) OVER(PARTITION BY Sensore ORDER BY DataOra, Sensore) 
	FROM ( 
		SELECT DataOra, Sensore, Valore = AVG(Valore) - @SOGLIA_TERMICA FROM DATA_CTE WHERE FunAggreg = 't_sum' GROUP BY DataOra, Sensore 
	) T 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = SUM(Valore) OVER(PARTITION BY Sensore ORDER BY DataOra, Sensore) 
	FROM ( 
		SELECT DataOra, Sensore, Valore = SUM(IIF(Valore < @SOGLIA_FABBISOGNO_FREDDO, 1, 0)) FROM DATA_CTE WHERE FunAggreg = 't_cnt_freddo' GROUP BY DataOra, Sensore 
	) T 
	UNION ALL 
	SELECT DataOra, Sensore, Valore = Dir FROM @TBL_AGGR_VET V INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = V.Id_SensoreDir
	UNION ALL 
	SELECT DataOra, Sensore, Valore = Vel FROM @TBL_AGGR_VET V INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = V.Id_SensoreVel

END 
ELSE 
BEGIN 

	INSERT INTO #Meteo 

	SELECT DataOra, Sensore, Valore 
	FROM @METEO_DATA d 
	INNER JOIN @SENSORS_OUT s ON s.Id_Sensore = d.Id_Sensore 
	WHERE s.FunAggreg <> 't_sum' 

	UNION ALL 

	SELECT DataOra, Sensore, Valore = SUM(IIF(0 > Valore, 0, Valore)) OVER(PARTITION BY Sensore ORDER BY DataOra, Sensore) 
	FROM ( 
		SELECT DataOra, Sensore, Valore = AVG(Valore) - @SOGLIA_TERMICA 
		FROM @METEO_DATA d 
		INNER JOIN @SENSORS_OUT s ON s.Id_Sensore = d.Id_Sensore 
		WHERE s.FunAggreg = 't_sum' 
		GROUP BY DataOra, Sensore 
	) T 
END 


DECLARE @columns AS VARCHAR(MAX) 

SELECT @columns = COALESCE(@columns + ', ', '') + QUOTENAME(TRIM(Sensore)) FROM @SENSORS_OUT ORDER BY Ordine

IF @columns IS NOT NULL 
BEGIN 

	--Tabella Dati 
	EXECUTE('SELECT DataOra, ' + @columns + ' FROM (SELECT DataOra, Sensore, Valore FROM #Meteo) x PIVOT (MAX(Valore) FOR Sensore IN (' + @columns + ')) p ORDER BY DataOra') 
	
	--Anagrafica colonne
	SELECT NomeColonna = Sensore, Sensore = Etichetta + DescrAggr, UM, TipoSensore = Id_TipoSensoreSrc, Misura = COALESCE(Misura, ''), FunAggreg, Configurazione = OutputConfig FROM @SENSORS_OUT ORDER BY Ordine 
	
	--Riepilogo 
	; WITH DATA_CTE AS (
		SELECT DataOra, Sensore, Valore, FunAggreg
		FROM @METEO_DATA D 
		INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = D.Id_Sensore
	)

	SELECT Sensore = Etichetta, FunAggreg, Valore, UM
	FROM ( 
		SELECT Sensore, Valore = AVG(Valore) FROM DATA_CTE WHERE FunAggreg = 'avg' GROUP BY Sensore 
		UNION 
		SELECT Sensore, Valore = MIN(Valore) FROM DATA_CTE WHERE FunAggreg = 'min' GROUP BY Sensore 
		UNION 
		SELECT Sensore, Valore = MAX(Valore) FROM DATA_CTE WHERE FunAggreg = 'max' GROUP BY Sensore 
		UNION 
		SELECT Sensore, Valore = SUM(Valore) FROM DATA_CTE WHERE FunAggreg = 'sum' GROUP BY Sensore
		UNION 
		SELECT Sensore, Valore = MAX(Valore) FROM (
			SELECT DataOra, Sensore, Valore = SUM(IIF(0 > Valore, 0, Valore)) OVER (PARTITION BY Sensore ORDER BY DataOra, Sensore) 
			FROM ( 
				SELECT DataOra, Sensore, Valore = AVG(Valore) - @SOGLIA_TERMICA FROM DATA_CTE
				WHERE FunAggreg = 't_sum' GROUP BY DataOra, Sensore 
			) T 
		) T GROUP BY Sensore 
		UNION 
		SELECT Sensore, Valore = SUM(IIF(Valore < @SOGLIA_FABBISOGNO_FREDDO, 1, 0)) FROM DATA_CTE WHERE FunAggreg = 't_cnt_freddo' GROUP BY Sensore 
	) T 
	INNER JOIN @SENSORS_OUT S ON S.Sensore = T.Sensore 
	WHERE S.Id_TipoSensoreSrc in (1, 2)
	ORDER BY S.Id_TipoSensoreSrc, S.Ordine

	--Riepilogo
	; WITH LAST_CTE AS ( 
		SELECT S.Sensore, Id_TipoSensoreSrc, FunAggreg, DataOra, val_last = Valore FROM ( 
			SELECT Id_Sensore, DataOra, Valore, Idx = ROW_NUMBER() OVER (PARTITION BY Id_Sensore ORDER BY DataOra DESC) 
			FROM @METEO_DATA 
		) D 
		INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = D.Id_Sensore
		WHERE S.Derived = 0 --Solo i sensori reali, non i derivati 
		AND Idx = 1 --Ultimo valore in ordine cronologico
	),
	AGGR_CTE AS (
		SELECT DataOra, Sensore, Valore, FunAggreg
		FROM @METEO_DATA D 
		INNER JOIN @SENSORS_OUT S ON S.Id_Sensore = D.Id_Sensore
	)

	SELECT 
		NomeColonna = S.Sensore
		, Etichetta
		, UM
		, DataOra
		, T1.val_last, T2.val_avg, T2.val_min, T2.val_max, T3.val_sum, T4.val_dir
	FROM LAST_CTE T1 
	LEFT JOIN ( 
		SELECT Sensore, val_avg = AVG(Valore), val_min = MIN(Valore), val_max = MAX(Valore) FROM AGGR_CTE WHERE FunAggreg = 'avg' GROUP BY Sensore 
	) T2 ON T2.Sensore = T1.Sensore 
	LEFT JOIN ( 
		SELECT Sensore, val_sum = SUM(Valore) FROM AGGR_CTE WHERE FunAggreg = 'sum' GROUP BY Sensore 
	) T3 ON T3.Sensore = T1.Sensore 
	LEFT JOIN ( 
		SELECT SensoreVel = TVel.Sensore, val_dir = L.val_last 
		FROM @WIND_MATCH T
		INNER JOIN @SENSORS_OUT TVel ON TVel.Id_Sensore = T.Id_SensoreVel
		INNER JOIN @SENSORS_OUT TDir ON TDir.Id_Sensore = T.Id_SensoreDir
		INNER JOIN LAST_CTE L ON L.Sensore = TDir.Sensore
	) T4 ON T4.SensoreVel = T1.Sensore 
	INNER JOIN @SENSORS_OUT S ON S.Sensore = T1.Sensore 
	WHERE T1.FunAggreg <> '' 
	ORDER BY S.Ordine

END 


BEGIN TRY
	DROP TABLE #Meteo
END TRY
BEGIN CATCH
END CATCH"

            Dim DS As New DataSet

            EseguiQuery_Lettura(ObjParametri_Server, sql, NomeRoutine, DS, "DatiMeteo_NT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return Nothing

#If False Then
    Public Function LeggiDati() As DataSet


        Try


            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti


                Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche


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
            sql.AppendLine("	SELECT Id, Misura, UM, FunAggreg, DefaultOutputConfig FROM MeteoNT_TipiSensore WITH (NOLOCK) WHERE Id = 0 ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Id, Misura = COALESCE(Misura, Tipo),  UM, FunAggreg, DefaultOutputConfig FROM MeteoNT_TipiSensore WITH (NOLOCK) WHERE Id <> 0 ")
            sql.AppendLine(") tipi ON tipi.Id = sxs.Id_TipoSensore ")
            sql.AppendLine("WHERE sxs.Id_Stazione = @STAZIONE ")
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
            sql.AppendLine("INNER JOIN MeteoNT_TipiSensore tipi WITH (NOLOCK) ON tipi.Id = conv.Id_TipoDst ")
            sql.AppendLine()
            sql.AppendLine()

            Dim tblDati As String = "MeteoNT_DatiOrari"
            Dim whereDati As String = ""

            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    Dim Tabella_RER As String = "Dati_Meteo_SHH"
                    Dim Campo_ID As String = "ID_Stazione"

                    If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                        Tabella_RER = "Dati_Meteo_QHH"
                        Campo_ID = "ID_Quadrante"
                    End If

                    sql.AppendLine("; WITH SRC_CTE AS ( ")
                    sql.AppendLine("    SELECT Tempo, Temp_Media, UmiditaRelativa, Bagnatura, Precipitazione FROM " & Tabella_RER & " WHERE " & Campo_ID & " = @STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE ")
                    sql.AppendLine("), ")
                    sql.AppendLine("DATI_RER AS ( ")
                    sql.AppendLine("	SELECT Id_Sensore = 1, DataOra = Tempo, Valore = Temp_Media FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 2, DataOra = Tempo, Valore = UmiditaRelativa FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 3, DataOra = Tempo, Valore = Bagnatura FROM SRC_CTE ")
                    sql.AppendLine("	UNION ")
                    sql.AppendLine("	SELECT Id_Sensore = 4, DataOra = Tempo, Valore = Precipitazione FROM SRC_CTE ")
                    sql.AppendLine(") ")

                    tblDati = "DATI_RER"

                Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche

                    If TipoSorgente <> enum_Meteo_Tiposorgente.Pubbliche AndAlso granularita = enum_GranularitaDati.Sorgente Then

                        tblDati = "MeteoNT_DatiSorgente"
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
            sql.AppendLine("	FROM " & tblDati & " dati WITH (NOLOCK) ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI sens ON sens.Id_Src = dati.Id_Sensore ")
            sql.AppendLine("	" & whereDati)
            sql.AppendLine(") D ")
            sql.AppendLine("INNER JOIN @TBL_SENSORI sens ON sens.Id = D.Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine()
            sql.Append(CreateTableDati("DatiMeteo"))
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
            sql.AppendLine("	INSERT INTO #DatiMeteo ")
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
            sql.AppendLine("	INSERT INTO #DatiMeteo ")
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
            sql.AppendLine("DELETE FROM @TBL_SENSORI WHERE Id NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) ")
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
            sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
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
            sql.AppendLine("			INNER JOIN MeteoNT_StazioniXSensori sxs1 WITH (NOLOCK) ON sxs1.Id_Sensore = TDir.Id_Src AND sxs1.FlagReale = 1 ")
            sql.AppendLine("			INNER JOIN MeteoNT_StazioniXSensori sxs2 WITH (NOLOCK) ON sxs2.Id_Sensore = TVel.Id_Src AND sxs2.FlagReale = 1 AND sxs1.Id_Stazione = sxs2.Id_Stazione ")
            sql.AppendLine("		) TT2 ")
            sql.AppendLine("		INNER JOIN Dati_CTE ON Dati_CTE.Id_Sensore = TT2.Id_SensoreDir ")
            sql.AppendLine("	) T4 ON T4.Id_Sensore = T1.Id_Sensore ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI S ON S.Id = T1.Id_Sensore ")
            sql.AppendLine("	WHERE T1.FunAggreg <> '' ")
            sql.AppendLine()
            sql.AppendLine("END ")
            sql.AppendLine()
            sql.Append(DropTableDati("DatiMeteo"))

            Dim DS As New DataSet

            EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine, DS, "DatiMeteo_NT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return Nothing
    End Function

#End If
    End Function





    Public Function LeggiDatiPerDSS(params As ParametriMeteoDSS, ObjParametri_Server As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerDSS"

        Try

            Dim id_stazione As String = "NULL"
            Dim id_quadrante_rer As String = "NULL"
            Dim id_stazione_rer As String = "NULL"

            Dim stazione_cod As String = Convert.ToString(params.Stazione, Globalization.CultureInfo.InvariantCulture)

            If params.Sorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                id_quadrante_rer = stazione_cod
            Else

                If params.Sorgente = enum_Meteo_Tiposorgente.Gias_RER Then

                    id_stazione_rer = stazione_cod
                Else

                    id_stazione = stazione_cod
                End If
            End If

            Dim sql As String

            sql = $"SET NOCOUNT ON 

DECLARE @ID_STAZIONE INT = {id_stazione};
DECLARE @ID_QUADRANTE_RER INT = {id_quadrante_rer};
DECLARE @ID_STAZIONE_RER INT = {id_stazione_rer};
DECLARE @STARTPERIOD DATETIME = '{params.DataInizio.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';
DECLARE @ENDPERIOD DATETIME = '{params.DataFine.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';
DECLARE @FORECAST BIT = {IIf(params.Forecast, "1", "0")};
DECLARE @SOGLIA_DISTANZA INTEGER = {params.SogliaDistanza_mt.ToString(CultureInfo.InvariantCulture)};
DECLARE @LW_LOWERBOUND_PERC REAL = {params.SogliaBagnaturaPerc.ToString(CultureInfo.InvariantCulture)};
DECLARE @LW_LOWERBOUND_TIME REAL = {params.SogliaBagnaturaMin.ToString(CultureInfo.InvariantCulture)};

DECLARE @LAT FLOAT = NULL;
DECLARE @LNG FLOAT = NULL;
DECLARE @NEED_PUBLIC BIT = 1;
DECLARE @ID_PUBLIC INTEGER = 0;
DECLARE @ID_FORECAST INTEGER = 0;
DECLARE @TIMEZONE AS VARCHAR(MAX) = 'W. Europe Standard Time';

DECLARE @SENSOR_IN TABLE(Id_Tipo INT, Id_Tipo_Out INT, LowerBound REAL, [Priority] INT, IsCumulated BIT)
INSERT INTO @SENSOR_IN VALUES
(1, 1000, NULL, 0, 0),					--Temperatura aria
(2, 2000, NULL, 0, 0),					--Pluviometro
(18, 2000, NULL, 1000, 1),				--Pluviometro cumulato
(3, 3000, NULL, 0, 0),					--Umidità relativa
(4, 4000, @LW_LOWERBOUND_PERC, 0, 0),	--Bagnatura fogliare superiore
(6, 4000, @LW_LOWERBOUND_PERC, 0, 0),	--Bagnatura fogliare superiore (%)
(22, 4000, @LW_LOWERBOUND_TIME, 0, 0),	--Bagnatura fogliare (min)
(35, 4000, 0, 0, 0),					--Bagnatura fogliare (h)
(42, 4000, 0, 0, 0)						--Bagnatura fogliare (0-1)


DECLARE @SENSOR_OUT TABLE(Id_Tipo INT, Des_Tipo VARCHAR(10), Flag BIT)
INSERT INTO @SENSOR_OUT VALUES (1000, 'Temp', 1), (2000, 'Prec', 1), (3000, 'RelHum', 1), (4000, 'LW', 1)

--(Scope = 0: Sensori stazione, Scope = 1: Sensori pubblici, Scope = 2: Sensori previsionali)
DECLARE @METEO_BY_SCOPE TABLE(DataOra DATETIME, Id_TipoSensore INT, Valore REAL, Scope INT)


---------------------------------------------------------------------------------------------------
--Se stazione Meteo_NT
---------------------------------------------------------------------------------------------------
SELECT TOP(1) @LAT = Lat, @LNG = Lng, @TIMEZONE = Timezone, @NEED_PUBLIC = NeedPublic
FROM (
	SELECT Lat = COALESCE(z1.Lat_Dec, z2.Lat_Dec), Lng = COALESCE(z1.Lng_Dec, z2.Lng_Dec), Timezone = COALESCE(z1.SystemTimeZoneInfo, z2.SystemTimeZoneInfo, @TIMEZONE), NeedPublic = IIF(z1.Categoria = 1, 0, 1)
	FROM (
		SELECT DISTINCT	Id_Src = z1.Id, Id_Reale = z2.Id
		FROM MeteoNT_Stazioni z1 WITH (NOLOCK)
		INNER JOIN MeteoNT_StazioniXSensori sxs1 WITH (NOLOCK) ON sxs1.Id_Stazione = z1.Id
		INNER JOIN MeteoNT_StazioniXSensori sxs2 WITH (NOLOCK) ON sxs2.Id_Sensore = sxs1.Id_Sensore AND sxs2.FlagReale = 1
		INNER JOIN MeteoNT_Stazioni z2 WITH (NOLOCK) ON z2.Id = sxs2.Id_Stazione
		WHERE z1.Id = @ID_STAZIONE
	) A
	INNER JOIN MeteoNT_Stazioni z1 WITH (NOLOCK) ON z1.Id = A.Id_Src
	INNER JOIN MeteoNT_Stazioni z2 WITH (NOLOCK) ON z2.Id = A.Id_Reale
) T
WHERE Lat IS NOT NULL AND Lng IS NOT NULL


DECLARE @SENSORI_X_DSS TABLE (Id INT, Id_TipoSensore INT)
INSERT INTO @SENSORI_X_DSS
SELECT Id, Id_TipoSensore FROM (
	SELECT Id, Id_TipoSensore, [Priority] = ROW_NUMBER() OVER (PARTITION BY Id_Tipo_Out ORDER BY Ordine)
	FROM (
		SELECT s.Id, s.Id_TipoSensore, Ordine = [Priority] + Ordine, Id_Tipo_Out
		FROM MeteoNT_Sensori s WITH (NOLOCK)
		INNER JOIN MeteoNT_StazioniXSensori x WITH (NOLOCK) ON x.Id_Sensore = s.Id
		INNER JOIN @SENSOR_IN i ON i.Id_Tipo = s.Id_TipoSensore
		WHERE x.Id_Stazione = @ID_STAZIONE 
	) T
) T WHERE [Priority] = 1


INSERT INTO @METEO_BY_SCOPE
SELECT 
	DataOra
	, Id_TipoSensore
	, Valore = IIF(PrecValore IS NULL, Valore, IIF(Valore < PrecValore, Valore, ROUND(Valore - precValore, 2)))
	, Scope = 0
FROM (
	SELECT DataOra, Id_TipoSensore, Valore, PrecValore = IIF(IsCumulated = 0, NULL, LAG(Valore, 1, NULL) OVER (PARTITION BY Id_Sensore ORDER BY Id_Sensore, DataOra))
	FROM MeteoNT_DatiOrari D WITH (NOLOCK)
	INNER JOIN @SENSORI_X_DSS X ON X.Id = D.Id_Sensore
	INNER JOIN @SENSOR_IN I ON I.Id_Tipo = X.Id_TipoSensore
	WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD
) T


---------------------------------------------------------------------------------------------------
--Se da Quadranti RER
---------------------------------------------------------------------------------------------------
SELECT @LAT = CAST(REPLACE(REPLACE(X_LON_GC, 'E', ''), ',', '.') AS FLOAT), @LNG = CAST(REPLACE(REPLACE(Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT)
FROM TB_Quadranti WITH (NOLOCK)
WHERE ID_Quadrante = @ID_QUADRANTE_RER

INSERT INTO @METEO_BY_SCOPE
SELECT DataOra, Id_TipoSensore, Valore, Scope = 0
FROM (
	SELECT DataOra = Tempo, [1] = Temp_Media, [2] = Precipitazione, [3] = CAST(Umiditarelativa AS REAL), [42] = CAST(Bagnatura AS REAL)
	FROM Dati_Meteo_QHH
	WHERE ID_Quadrante = @ID_QUADRANTE_RER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD
) T UNPIVOT (Valore FOR Id_TipoSensore IN ([1], [2], [3], [42])) UNPVT


---------------------------------------------------------------------------------------------------
-- Se da Stazioni RER
---------------------------------------------------------------------------------------------------
SELECT @LAT = CAST(REPLACE(REPLACE(Q.X_LON_GC, 'E', ''), ',', '.') AS FLOAT), @LNG = CAST(REPLACE(REPLACE(Q.Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT)
FROM TB_QuadrantixStazioni QS WITH (NOLOCK)
INNER JOIN TB_Quadranti Q WITH (NOLOCK) ON Q.ID_Quadrante = QS.ID_Quadrante
WHERE QS.ID_Stazione = @ID_STAZIONE_RER

INSERT INTO @METEO_BY_SCOPE
SELECT DataOra, Id_TipoSensore, Valore, Scope = 0--, Ordine = Id_TipoSensore
FROM (
	SELECT DataOra = Tempo, [1] = Temp_Media, [2] = Precipitazione, [3] = CAST(Umiditarelativa AS REAL), [42] = CAST(Bagnatura AS REAL)
	FROM Dati_Meteo_SHH
	WHERE ID_Stazione = @ID_STAZIONE_RER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD
) T UNPIVOT (Valore FOR Id_TipoSensore IN ([1], [2], [3], [42])) UNPVT


--Ignoro i sensori che in origine non esistono
UPDATE O SET Flag = 0
FROM @SENSOR_OUT O
LEFT JOIN
(
	SELECT Id_Tipo_Out
	FROM MeteoNT_Sensori S WITH (NOLOCK)
	INNER JOIN MeteoNT_StazioniXSensori SXS WITH (NOLOCK) ON SXS.Id_Sensore = S.Id AND SXS.Id_Stazione = @ID_STAZIONE
	INNER JOIN @SENSOR_IN I ON I.Id_Tipo = S.Id_TipoSensore
	UNION ALL
	SELECT Id_Tipo_Out
	FROM  (
	VALUES
		(@ID_QUADRANTE_RER, 1), (@ID_QUADRANTE_RER, 2), (@ID_QUADRANTE_RER, 3), (@ID_QUADRANTE_RER, 42),
		(@ID_STAZIONE_RER, 1), (@ID_STAZIONE_RER, 2), (@ID_STAZIONE_RER, 3), (@ID_STAZIONE_RER, 42)
	) T (Id_Stazione, Id_TipoSensore)
	INNER JOIN @SENSOR_IN I ON I.Id_Tipo = T.Id_TipoSensore
	WHERE Id_Stazione IS NOT NULL AND Id_Stazione <> 0
) T ON T.Id_Tipo_Out = O.Id_Tipo
WHERE T.Id_Tipo_Out IS NULL



IF (@LAT IS NOT NULL AND @LNG IS NOT NULL)
BEGIN

	DECLARE @REQUEST_POINT GEOGRAPHY = GEOGRAPHY::Point(@LAT, @LNG, 4326)

	IF (@NEED_PUBLIC = 1)
	BEGIN

		SELECT TOP (1) @ID_PUBLIC = Id
		FROM MeteoNT_Stazioni WITH (NOLOCK)
		WHERE Categoria = 1 AND GeoEntity.STDistance(@REQUEST_POINT) < @SOGLIA_DISTANZA
		ORDER BY GeoEntity.STDistance(@REQUEST_POINT)

		INSERT INTO @METEO_BY_SCOPE
		SELECT DataOra, Id_TipoSensore, Valore, Scope = 1--, Ordine
		FROM MeteoNT_DatiOrari D WITH (NOLOCK)
		INNER JOIN MeteoNT_Sensori S WITH (NOLOCK) ON S.Id = D.Id_Sensore
		INNER JOIN MeteoNT_StazioniXSensori SXS WITH (NOLOCK) ON SXS.Id_Sensore = S.Id
		INNER JOIN @SENSOR_IN I ON I.Id_Tipo = S.Id_TipoSensore
		INNER JOIN @SENSOR_OUT O ON O.Id_Tipo = I.Id_Tipo_Out AND O.Flag = 1
		WHERE Id_Stazione = @ID_PUBLIC AND @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD

	END

	IF (@FORECAST = 1)
	BEGIN

		IF (EXISTS (SELECT TOP(1) DataOra FROM @METEO_BY_SCOPE))
		BEGIN

			SELECT TOP (1) @ID_FORECAST = Id
			FROM Hypermeteo_HFS_ITA_4KM WITH (NOLOCK)
			WHERE Cell.STDistance(@REQUEST_POINT) < @SOGLIA_DISTANZA
			ORDER BY Cell.STDistance(@REQUEST_POINT)

			INSERT INTO @METEO_BY_SCOPE
			SELECT DataOra, Id_TipoSensore, Valore, Scope = 2--, Ordine
			FROM (
				SELECT DataOra = CAST(DataOraUTC AT TIME ZONE 'UTC' AT TIME ZONE 'W. Europe Standard Time' AS DATETIME), Id_TipoSensore, Valore--, Ordine
				FROM Hypermeteo_FORECAST_METEO_Dati D WITH (NOLOCK)
				INNER JOIN (
					SELECT Id_Sensore = S.Id, Id_TipoSensore = T.IdSource--, Ordine = S.Id_TipoSensore
					FROM Hypermeteo_FORECAST_METEO_Sensori S WITH (NOLOCK)
					INNER JOIN Hypermeteo_FORECAST_METEO_TipiSensore T WITH (NOLOCK) ON T.Id = S.Id_TipoSensore
					INNER JOIN @SENSOR_IN I ON I.Id_Tipo = T.IdSource
					INNER JOIN @SENSOR_OUT O ON O.Id_Tipo = I.Id_Tipo_Out AND O.Flag = 1
					WHERE Id_Stazione = @ID_FORECAST
				) T ON T.Id_Sensore = D.Id_Sensore
			) T
			WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD

		END
	END
END




BEGIN TRY
	DROP TABLE #DataTbl
END TRY
BEGIN CATCH
END CATCH

CREATE TABLE #DataTbl (DataOra DATETIME, TipoSensore INT, Valore REAL, Scope INTEGER)
INSERT INTO #DataTbl
SELECT DataOra, TipoSensore, Valore, Scope
FROM (
	SELECT DataOra, TipoSensore, Valore, Scope, RowNum = ROW_NUMBER() OVER (PARTITION BY DataOra, TipoSensore ORDER BY DataOra, TipoSensore, Scope)
	FROM (
		SELECT DataOra, TipoSensore = Id_Tipo_Out, Valore = IIF(LowerBound IS NULL, Valore, IIF(Valore > LowerBound, 1, 0)), Scope
		FROM @METEO_BY_SCOPE M
		INNER JOIN @SENSOR_IN I ON I.Id_Tipo = M.Id_TipoSensore
	) T
) T WHERE RowNum = 1


UPDATE O SET Flag = IIF(D.TipoSensore IS NULL, 0, 1)
FROM @SENSOR_OUT O
LEFT JOIN (
	SELECT DISTINCT TipoSensore FROM #DataTbl
) D ON D.TipoSensore = O.Id_Tipo

DECLARE @columns VARCHAR(MAX), @pivot VARCHAR(MAX)

SELECT
	@columns = COALESCE(@columns + ', ', '') + QUOTENAME(TRIM(Des_Tipo)) + ' = MIN(' + QUOTENAME(TRIM(CAST(Id_Tipo AS VARCHAR(5)))) + ')'
	, @pivot = COALESCE(@pivot + ', ', '') + QUOTENAME(TRIM(CAST(Id_Tipo AS VARCHAR(5))))
FROM @SENSOR_OUT WHERE Flag = 1

IF (@pivot IS NOT NULL)
BEGIN
	EXECUTE('SELECT DataOra, ' + @columns + ', Forecast = IIF(MIN(Scope) < 2, 0, 1) FROM #DataTbl PIVOT (MAX(Valore) FOR TipoSensore IN (' + @pivot + ')) PVT GROUP BY DataOra ORDER BY DataOra')

	SELECT Sensor = Des_Tipo, Found = Flag FROM @SENSOR_OUT

	SELECT Lat = @LAT, Lng = @LNG, Timezone = @TIMEZONE
END

BEGIN TRY
	DROP TABLE #DataTbl
END TRY
BEGIN CATCH
END CATCH"

            'Dim sql As New StringBuilder

            'sql.Clear()
            'sql.AppendLine("SET NOCOUNT ON")
            'sql.AppendLine()
            'sql.AppendLine($"DECLARE @ID_STAZIONE INT = {id_stazione};")
            'sql.AppendLine($"DECLARE @ID_QUADRANTE_RER INT = {id_quadrante_rer};")
            'sql.AppendLine($"DECLARE @ID_STAZIONE_RER INT = {id_stazione_rer};")
            'sql.AppendLine($"DECLARE @STARTPERIOD DATETIME = '{params.DataInizio.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';")
            'sql.AppendLine($"DECLARE @ENDPERIOD DATETIME = '{params.DataFine.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';")
            'sql.AppendLine($"DECLARE @FORECAST BIT = {IIf(params.Forecast, "1", "0")};")
            'sql.AppendLine($"DECLARE @SOGLIA_DISTANZA INTEGER = {params.SogliaDistanza_mt.ToString(CultureInfo.InvariantCulture)};")
            'sql.AppendLine($"DECLARE @LW_LOWERBOUND_PERC REAL = {params.SogliaBagnaturaPerc.ToString(CultureInfo.InvariantCulture)};")
            'sql.AppendLine($"DECLARE @LW_LOWERBOUND_TIME REAL = {params.SogliaBagnaturaMin.ToString(CultureInfo.InvariantCulture)};")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @LAT FLOAT = NULL;")
            'sql.AppendLine("DECLARE @LNG FLOAT = NULL;")
            'sql.AppendLine("DECLARE @NEED_PUBLIC BIT = 1;")
            'sql.AppendLine("DECLARE @ID_PUBLIC INTEGER = 0;")
            'sql.AppendLine("DECLARE @ID_FORECAST INTEGER = 0;")
            'sql.AppendLine("DECLARE @TIMEZONE AS VARCHAR(MAX) = 'W. Europe Standard Time';")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @SENSOR_IN TABLE(Id_Tipo INT, Id_Tipo_Out INT, LowerBound REAL)")
            'sql.AppendLine("INSERT INTO @SENSOR_IN VALUES")
            'sql.AppendLine("(1, 1000, NULL),					--Temperatura aria")
            'sql.AppendLine("(2, 2000, NULL),					--Pluviometro")
            'sql.AppendLine("(3, 3000, NULL),					--Umidità relativa")
            'sql.AppendLine("(4, 4000, @LW_LOWERBOUND_PERC),		--Bagnatura fogliare superiore")
            'sql.AppendLine("(6, 4000, @LW_LOWERBOUND_PERC),		--Bagnatura fogliare superiore (%)")
            'sql.AppendLine("(22, 4000, @LW_LOWERBOUND_TIME),	--Bagnatura fogliare (min)")
            'sql.AppendLine("(35, 4000, 0),						--Bagnatura fogliare (h)")
            'sql.AppendLine("(42, 4000, 0)						--Bagnatura fogliare (0-1)")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @SENSOR_OUT TABLE(Id_Tipo INT, Des_Tipo VARCHAR(10), Flag BIT)")
            'sql.AppendLine("INSERT INTO @SENSOR_OUT VALUES (1000, 'Temp', 1), (2000, 'Prec', 1), (3000, 'RelHum', 1), (4000, 'LW', 1)")
            'sql.AppendLine()
            'sql.AppendLine("--(Scope = 0: Sensori stazione, Scope = 1: Sensori pubblici, Scope = 2: Sensori previsionali)")
            'sql.AppendLine("DECLARE @METEO_BY_SCOPE TABLE(DataOra DATETIME, Id_TipoSensore INT, Valore REAL, Scope INT, Ordine INT)")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("--Se stazione Meteo_NT")
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("SELECT TOP(1) @LAT = Lat, @LNG = Lng, @TIMEZONE = Timezone, @NEED_PUBLIC = NeedPublic")
            'sql.AppendLine("FROM (")
            'sql.AppendLine("	SELECT Lat = COALESCE(z1.Lat_Dec, z2.Lat_Dec), Lng = COALESCE(z1.Lng_Dec, z2.Lng_Dec), Timezone = COALESCE(z1.SystemTimeZoneInfo, z2.SystemTimeZoneInfo, @TIMEZONE), NeedPublic = IIF(z1.Categoria = 1, 0, 1)")
            'sql.AppendLine("	FROM (")
            'sql.AppendLine("		SELECT DISTINCT	Id_Src = z1.Id, Id_Reale = z2.Id")
            'sql.AppendLine("		FROM MeteoNT_Stazioni z1 WITH (NOLOCK)")
            'sql.AppendLine("		INNER JOIN MeteoNT_StazioniXSensori sxs1 WITH (NOLOCK) ON sxs1.Id_Stazione = z1.Id")
            'sql.AppendLine("		INNER JOIN MeteoNT_StazioniXSensori sxs2 WITH (NOLOCK) ON sxs2.Id_Sensore = sxs1.Id_Sensore AND sxs2.FlagReale = 1")
            'sql.AppendLine("		INNER JOIN MeteoNT_Stazioni z2 WITH (NOLOCK) ON z2.Id = sxs2.Id_Stazione")
            'sql.AppendLine("		WHERE z1.Id = @ID_STAZIONE")
            'sql.AppendLine("	) A")
            'sql.AppendLine("	INNER JOIN MeteoNT_Stazioni z1 WITH (NOLOCK) ON z1.Id = A.Id_Src")
            'sql.AppendLine("	INNER JOIN MeteoNT_Stazioni z2 WITH (NOLOCK) ON z2.Id = A.Id_Reale")
            'sql.AppendLine(") T")
            'sql.AppendLine("WHERE Lat IS NOT NULL AND Lng IS NOT NULL")
            'sql.AppendLine()
            'sql.AppendLine("INSERT INTO @METEO_BY_SCOPE")
            'sql.AppendLine("SELECT DataOra, Id_TipoSensore, Valore, Scope = 0, Ordine")
            'sql.AppendLine("FROM MeteoNT_DatiOrari D WITH (NOLOCK)")
            'sql.AppendLine("INNER JOIN MeteoNT_Sensori S WITH (NOLOCK) ON S.Id = D.Id_Sensore")
            'sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori SXS WITH (NOLOCK) ON SXS.Id_Sensore = S.Id")
            'sql.AppendLine("INNER JOIN @SENSOR_IN I ON I.Id_Tipo = S.Id_TipoSensore")
            'sql.AppendLine("WHERE Id_Stazione = @ID_STAZIONE AND @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("--Se da Quadranti RER")
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("SELECT @LAT = CAST(REPLACE(REPLACE(X_LON_GC, 'E', ''), ',', '.') AS FLOAT), @LNG = CAST(REPLACE(REPLACE(Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT)")
            'sql.AppendLine("FROM TB_Quadranti WITH (NOLOCK)")
            'sql.AppendLine("WHERE ID_Quadrante = @ID_QUADRANTE_RER")
            'sql.AppendLine()
            'sql.AppendLine("INSERT INTO @METEO_BY_SCOPE")
            'sql.AppendLine("SELECT DataOra, Id_TipoSensore, Valore, Scope = 0, Ordine = Id_TipoSensore")
            'sql.AppendLine("FROM (")
            'sql.AppendLine("	SELECT DataOra = Tempo, [1] = Temp_Media, [2] = Precipitazione, [3] = CAST(Umiditarelativa AS REAL), [42] = CAST(Bagnatura AS REAL)")
            'sql.AppendLine("	FROM Dati_Meteo_QHH")
            'sql.AppendLine("	WHERE ID_Quadrante = @ID_QUADRANTE_RER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD")
            'sql.AppendLine(") T UNPIVOT (Valore FOR Id_TipoSensore IN ([1], [2], [3], [42])) UNPVT")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("-- Se da Stazioni RER")
            'sql.AppendLine("---------------------------------------------------------------------------------------------------")
            'sql.AppendLine("SELECT @LAT = CAST(REPLACE(REPLACE(Q.X_LON_GC, 'E', ''), ',', '.') AS FLOAT), @LNG = CAST(REPLACE(REPLACE(Q.Y_LAT_GC, 'N', ''), ',', '.') AS FLOAT)")
            'sql.AppendLine("FROM TB_QuadrantixStazioni QS WITH (NOLOCK)")
            'sql.AppendLine("INNER JOIN TB_Quadranti Q WITH (NOLOCK) ON Q.ID_Quadrante = QS.ID_Quadrante")
            'sql.AppendLine("WHERE QS.ID_Stazione = @ID_STAZIONE_RER")
            'sql.AppendLine("")
            'sql.AppendLine("INSERT INTO @METEO_BY_SCOPE")
            'sql.AppendLine("SELECT DataOra, Id_TipoSensore, Valore, Scope = 0, Ordine = Id_TipoSensore")
            'sql.AppendLine("FROM (")
            'sql.AppendLine("	SELECT DataOra = Tempo, [1] = Temp_Media, [2] = Precipitazione, [3] = CAST(Umiditarelativa AS REAL), [42] = CAST(Bagnatura AS REAL)")
            'sql.AppendLine("	FROM Dati_Meteo_SHH")
            'sql.AppendLine("	WHERE ID_Stazione = @ID_STAZIONE_RER AND @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD")
            'sql.AppendLine(") T UNPIVOT (Valore FOR Id_TipoSensore IN ([1], [2], [3], [42])) UNPVT")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("--Ignoro i sensori che in origine non esistono")
            'sql.AppendLine("UPDATE O SET Flag = 0")
            'sql.AppendLine("FROM @SENSOR_OUT O")
            'sql.AppendLine("LEFT JOIN")
            'sql.AppendLine("(")
            'sql.AppendLine("	SELECT Id_Tipo_Out")
            'sql.AppendLine("	FROM MeteoNT_Sensori S WITH (NOLOCK)")
            'sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori SXS WITH (NOLOCK) ON SXS.Id_Sensore = S.Id AND SXS.Id_Stazione = @ID_STAZIONE")
            'sql.AppendLine("	INNER JOIN @SENSOR_IN I ON I.Id_Tipo = S.Id_TipoSensore")
            'sql.AppendLine("	UNION ALL")
            'sql.AppendLine("	SELECT Id_Tipo_Out")
            'sql.AppendLine("	FROM  (")
            'sql.AppendLine("	VALUES")
            'sql.AppendLine("		(@ID_QUADRANTE_RER, 1), (@ID_QUADRANTE_RER, 2), (@ID_QUADRANTE_RER, 3), (@ID_QUADRANTE_RER, 42),")
            'sql.AppendLine("		(@ID_STAZIONE_RER, 1), (@ID_STAZIONE_RER, 2), (@ID_STAZIONE_RER, 3), (@ID_STAZIONE_RER, 42)")
            'sql.AppendLine("	) T (Id_Stazione, Id_TipoSensore)")
            'sql.AppendLine("	INNER JOIN @SENSOR_IN I ON I.Id_Tipo = T.Id_TipoSensore")
            'sql.AppendLine("	WHERE Id_Stazione IS NOT NULL AND Id_Stazione <> 0")
            'sql.AppendLine(") T ON T.Id_Tipo_Out = O.Id_Tipo")
            'sql.AppendLine("WHERE T.Id_Tipo_Out IS NULL")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("IF (@LAT IS NOT NULL AND @LNG IS NOT NULL)")
            'sql.AppendLine("BEGIN")
            'sql.AppendLine()
            'sql.AppendLine("	DECLARE @REQUEST_POINT GEOGRAPHY = GEOGRAPHY::Point(@LAT, @LNG, 4326)")
            'sql.AppendLine()
            'sql.AppendLine("	IF (@NEED_PUBLIC = 1)")
            'sql.AppendLine("	BEGIN")
            'sql.AppendLine()
            'sql.AppendLine("		SELECT @ID_PUBLIC = Id")
            'sql.AppendLine("		FROM (")
            'sql.AppendLine("		    SELECT TOP (1) Id, Distanza = GeoEntity.STDistance(@REQUEST_POINT)")
            'sql.AppendLine("			FROM MeteoNT_Stazioni WITH (NOLOCK)")
            'sql.AppendLine("			WHERE Categoria = 1")
            'sql.AppendLine("			ORDER BY Distanza")
            'sql.AppendLine("		) T WHERE Distanza < @SOGLIA_DISTANZA")
            'sql.AppendLine()
            'sql.AppendLine("		INSERT INTO @METEO_BY_SCOPE")
            'sql.AppendLine("		SELECT DataOra, Id_TipoSensore, Valore, Scope = 1, Ordine")
            'sql.AppendLine("		FROM MeteoNT_DatiOrari D WITH (NOLOCK)")
            'sql.AppendLine("		INNER JOIN MeteoNT_Sensori S WITH (NOLOCK) ON S.Id = D.Id_Sensore")
            'sql.AppendLine("		INNER JOIN MeteoNT_StazioniXSensori SXS WITH (NOLOCK) ON SXS.Id_Sensore = S.Id")
            'sql.AppendLine("		INNER JOIN @SENSOR_IN I ON I.Id_Tipo = S.Id_TipoSensore")
            'sql.AppendLine("		INNER JOIN @SENSOR_OUT O ON O.Id_Tipo = I.Id_Tipo_Out AND O.Flag = 1")
            'sql.AppendLine("		WHERE Id_Stazione = @ID_PUBLIC AND @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD")
            'sql.AppendLine()
            'sql.AppendLine("	END")
            'sql.AppendLine()
            'sql.AppendLine("	IF (@FORECAST = 1)")
            'sql.AppendLine("	BEGIN")
            'sql.AppendLine()
            'sql.AppendLine("		IF (EXISTS (SELECT TOP(1) DataOra FROM @METEO_BY_SCOPE))")
            'sql.AppendLine("		BEGIN")
            'sql.AppendLine()
            'sql.AppendLine("			SELECT @ID_FORECAST = Id")
            'sql.AppendLine("			FROM (")
            'sql.AppendLine("				SELECT TOP (1) Id, Distanza = Cell.STDistance(@REQUEST_POINT)")
            'sql.AppendLine("				FROM Hypermeteo_HFS_ITA_4KM WITH (NOLOCK)")
            'sql.AppendLine("				ORDER BY Distanza")
            'sql.AppendLine("			) T WHERE Distanza < @SOGLIA_DISTANZA")
            'sql.AppendLine()
            'sql.AppendLine("			INSERT INTO @METEO_BY_SCOPE")
            'sql.AppendLine("			SELECT DataOra, Id_TipoSensore, Valore, Scope = 2, Ordine")
            'sql.AppendLine("			FROM (")
            'sql.AppendLine("				SELECT DataOra = CAST(DataOraUTC AT TIME ZONE 'UTC' AT TIME ZONE 'W. Europe Standard Time' AS DATETIME), Id_TipoSensore, Valore, Ordine")
            'sql.AppendLine("				FROM Hypermeteo_FORECAST_METEO_Dati D WITH (NOLOCK)")
            'sql.AppendLine("				INNER JOIN (")
            'sql.AppendLine("					SELECT Id_Sensore = S.Id, Id_TipoSensore = T.IdSource, Ordine = S.Id_TipoSensore")
            'sql.AppendLine("					FROM Hypermeteo_FORECAST_METEO_Sensori S WITH (NOLOCK)")
            'sql.AppendLine("					INNER JOIN Hypermeteo_FORECAST_METEO_TipiSensore T WITH (NOLOCK) ON T.Id = S.Id_TipoSensore")
            'sql.AppendLine("					INNER JOIN @SENSOR_IN I ON I.Id_Tipo = T.IdSource")
            'sql.AppendLine("					INNER JOIN @SENSOR_OUT O ON O.Id_Tipo = I.Id_Tipo_Out AND O.Flag = 1")
            'sql.AppendLine("					WHERE Id_Stazione = @ID_FORECAST")
            'sql.AppendLine("				) T ON T.Id_Sensore = D.Id_Sensore")
            'sql.AppendLine("			) T")
            'sql.AppendLine("			WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD")
            'sql.AppendLine()
            'sql.AppendLine("		END")
            'sql.AppendLine("	END")
            'sql.AppendLine("END")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("BEGIN TRY")
            'sql.AppendLine("	DROP TABLE #DataTbl")
            'sql.AppendLine("END TRY")
            'sql.AppendLine("BEGIN CATCH")
            'sql.AppendLine("END CATCH")
            'sql.AppendLine()
            'sql.AppendLine("CREATE TABLE #DataTbl (DataOra DATETIME, TipoSensore INT, Valore REAL, Scope INTEGER)")
            'sql.AppendLine("INSERT INTO #DataTbl")
            'sql.AppendLine("SELECT DataOra, TipoSensore, Valore, Scope")
            'sql.AppendLine("FROM (")
            'sql.AppendLine("	SELECT DataOra, TipoSensore, Valore, Scope, RowNum = ROW_NUMBER() OVER (PARTITION BY DataOra, TipoSensore ORDER BY DataOra, TipoSensore, Scope, Ordine)")
            'sql.AppendLine("	FROM (")
            'sql.AppendLine("		SELECT DataOra, TipoSensore = Id_Tipo_Out, Valore = IIF(LowerBound IS NULL, Valore, IIF(Valore > LowerBound, 1, 0)), Ordine, Scope")
            'sql.AppendLine("		FROM @METEO_BY_SCOPE M")
            'sql.AppendLine("		INNER JOIN @SENSOR_IN I ON I.Id_Tipo = M.Id_TipoSensore")
            'sql.AppendLine("	) T")
            'sql.AppendLine(") T WHERE RowNum = 1")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("UPDATE O SET Flag = IIF(D.TipoSensore IS NULL, 0, 1)")
            'sql.AppendLine("FROM @SENSOR_OUT O")
            'sql.AppendLine("LEFT JOIN (")
            'sql.AppendLine("	SELECT DISTINCT TipoSensore FROM #DataTbl")
            'sql.AppendLine(") D ON D.TipoSensore = O.Id_Tipo")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @columns VARCHAR(MAX), @pivot VARCHAR(MAX)")
            'sql.AppendLine()
            'sql.AppendLine("SELECT")
            'sql.AppendLine("	@columns = COALESCE(@columns + ', ', '') + QUOTENAME(TRIM(Des_Tipo)) + ' = MIN(' + QUOTENAME(TRIM(CAST(Id_Tipo AS VARCHAR(5)))) + ')'")
            'sql.AppendLine("	, @pivot = COALESCE(@pivot + ', ', '') + QUOTENAME(TRIM(CAST(Id_Tipo AS VARCHAR(5))))")
            'sql.AppendLine("FROM @SENSOR_OUT WHERE Flag = 1")
            'sql.AppendLine()
            'sql.AppendLine("IF (@pivot IS NOT NULL)")
            'sql.AppendLine("BEGIN")
            'sql.AppendLine("	EXECUTE('SELECT DataOra, ' + @columns + ', Forecast = IIF(MIN(Scope) < 2, 0, 1) FROM #DataTbl PIVOT (MAX(Valore) FOR TipoSensore IN (' + @pivot + ')) PVT GROUP BY DataOra ORDER BY DataOra')")
            'sql.AppendLine()
            'sql.AppendLine("	SELECT Sensor = Des_Tipo, Found = Flag FROM @SENSOR_OUT")
            'sql.AppendLine()
            'sql.AppendLine("	SELECT Lat = @LAT, Lng = @LNG, Timezone = @TIMEZONE")
            'sql.AppendLine("END")
            'sql.AppendLine()
            'sql.AppendLine("BEGIN TRY")
            'sql.AppendLine("	DROP TABLE #DataTbl")
            'sql.AppendLine("END TRY")
            'sql.AppendLine("BEGIN CATCH")
            'sql.AppendLine("END CATCH")

            Dim DS As New DataSet

            EseguiQuery_Lettura(ObjParametri_Server, sql, NomeRoutine, DS, "DatiMeteo_NT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return Nothing
    End Function


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

    Public Function LeggiDatiPerPiogge(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer,
                                       DataInizio As DateTime, DataFine As DateTime,
                                       ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerPiogge"
        Dim DT As DataTable

        Try
            Dim sql As String = $"SET NOCOUNT ON 

DECLARE @ID_STAZIONE INT = {IdStazione};
DECLARE @STARTPERIOD DATETIME = '{DataInizio.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';
DECLARE @ENDPERIOD DATETIME = '{DataFine.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';"

            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                Dim tbl_dati_meteo As String = "Dati_Meteo_QHH"
                Dim field_id As String = "ID_Quadrante"
                If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then
                    tbl_dati_meteo = "Dati_Meteo_SHH"
                    field_id = "ID_Stazione"
                End If

                sql += $"
SELECT 
	DataOra = Giorno
	, Temp = ROUND(AVG(Temp_Media), 2)
	, TempMin = MIN(Temp_Media)
	, TempMax = MAX(Temp_Media)
	, UmRel = ROUND(AVG(UmiditaRelativa), 2)
	, Prec = ROUND(SUM(Precipitazione), 2)
FROM (
	SELECT Giorno = DATEADD(dd, 0, DATEDIFF(dd, 0, Tempo)), Temp_Media, Precipitazione, UmiditaRelativa
	FROM {tbl_dati_meteo} WITH (NOLOCK)		
	WHERE @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD AND {field_id} = @ID_STAZIONE 
) T GROUP BY Giorno ORDER BY Giorno"

            Else

                sql += $"
DECLARE @SENSOR_FLT TABLE(Id_TipoSensore INT, Category INT, [Priority] INT, IsCumulated BIT) 
INSERT INTO @SENSOR_FLT VALUES 
(1, 1, 1, 0),	--Temperatura Aria
(3, 2, 1, 0),	--Umidità Relativa
(2, 3, 1, 0),	--Pluviometro 
(18, 3, 2, 1)	--Pluviometro cumulato



DECLARE @AGGR_TBL TABLE (Id_TipoSensoreSrc INT, FunAggreg VARCHAR(50), [Label] VARCHAR(50))
INSERT INTO @AGGR_TBL VALUES
(1, 'avg', 'Temp'),
(2, 'sum', 'Prec'),
(18, 'sum', 'Prec'),
(3, 'avg', 'UmRel'), 
(1, 'min', 'TempMin'),
(1, 'max', 'TempMax')



DECLARE @SRC_SENSOR TABLE(Id_Sensore INT, Id_TipoSensore INT, IsCumulated BIT) 
INSERT INTO @SRC_SENSOR
SELECT Id_Sensore, Id_TipoSensore, IsCumulated from (
	SELECT 
		Id_Sensore = s.Id 
		, Id_TipoSensore = s.Id_TipoSensore
		, IsCumulated
		, Category
		, [Priority]
		, Flag_Tipo = ROW_NUMBER() OVER (PARTITION BY s.Id_TipoSensore ORDER BY Ordine)
		, Flag_Gategory = ROW_NUMBER() OVER (PARTITION BY Category ORDER BY [Priority])
	FROM MeteoNT_Sensori s WITH (NOLOCK) 
	INNER JOIN MeteoNT_StazioniXSensori x WITH (NOLOCK) ON x.Id_Sensore = s.Id
	INNER JOIN @SENSOR_FLT f ON f.Id_TipoSensore = s.Id_TipoSensore
	WHERE x.Id_Stazione = @ID_STAZIONE
) T WHERE (Flag_Tipo = 1 AND Flag_Gategory = 1) OR [Priority] = 0


DECLARE @SRC_DATI TABLE(Giorno DATETIME, Id_Sensore INT, Id_TipoSensore INT, Valore REAL) 
INSERT INTO @SRC_DATI 
SELECT 
	Giorno = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra))
	, Id_Sensore
	, Id_TipoSensore
	, Valore = IIF(PrecValore IS NULL, Valore, IIF(Valore < PrecValore, Valore, ROUND(Valore - precValore, 2)))
FROM (
	SELECT DataOra, d.Id_Sensore, Id_TipoSensore, Valore, PrecValore = IIF(IsCumulated = 0, NULL, LAG(Valore, 1, NULL) OVER (PARTITION BY d.Id_Sensore ORDER BY d.Id_Sensore, DataOra))
	FROM MeteoNT_DatiOrari d WITH (NOLOCK) 
	INNER JOIN @SRC_SENSOR s ON s.Id_Sensore = d.Id_Sensore 
	WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD
) T


DECLARE @SENSORS TABLE(Id_SensoreSrc INT, Id_SensoreDst INT IDENTITY, [Column] VARCHAR(50), [Label] VARCHAR(MAX), FunAggreg VARCHAR(50))
INSERT INTO @SENSORS
SELECT s.Id, COALESCE(a.[Label], 'UT_' + CAST(s.Id AS VARCHAR(10))), COALESCE(x.Etichetta, s.Etichetta), FunAggreg
FROM @SRC_SENSOR ss
INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) ON s.Id = ss.Id_Sensore
INNER JOIN MeteoNT_StazioniXSensori x WITH (NOLOCK) ON x.Id_Sensore = s.Id AND x.Id_Stazione = @ID_STAZIONE
INNER JOIN @AGGR_TBL a ON a.Id_TipoSensoreSrc = s.Id_TipoSensore


BEGIN TRY
    DROP TABLE #DatiMeteo
END TRY
BEGIN CATCH
END CATCH

CREATE TABLE #DatiMeteo (Giorno DATETIME, Id_Sensore INT, Valore REAL) 

;WITH DATI_CTE AS (
	SELECT Giorno, Id_Sensore = Id_SensoreDst, Valore, FunAggreg
	FROM @SRC_DATI d
	INNER JOIN @SENSORS a ON a.Id_SensoreSrc = d.Id_Sensore
)

INSERT INTO #DatiMeteo
SELECT Giorno, Id_Sensore, Valore = AVG(Valore) FROM DATI_CTE WHERE FunAggreg = 'avg' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = MIN(Valore) FROM DATI_CTE WHERE FunAggreg = 'min' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = MAX(Valore) FROM DATI_CTE WHERE FunAggreg = 'max' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = SUM(Valore) FROM DATI_CTE WHERE FunAggreg = 'sum' GROUP BY Giorno, Id_Sensore


DELETE FROM @SENSORS WHERE Id_SensoreDst NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo)

DECLARE @columns VARCHAR(MAX), @pivot VARCHAR(MAX)

SELECT
	@columns = COALESCE(@columns + ', ', '') + QUOTENAME(TRIM([Column])) + ' = ' + QUOTENAME(TRIM(CAST(Id_SensoreDst AS VARCHAR(5))))
	, @pivot = COALESCE(@pivot + ', ', '') + QUOTENAME(TRIM(CAST(Id_SensoreDst AS VARCHAR(5))))
FROM @SENSORS

IF @columns IS NOT NULL 
BEGIN 

    EXECUTE('SELECT Giorno AS [DataOra], ' + @columns + ' FROM (SELECT Giorno, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @pivot + ')) p ORDER BY Giorno') 

END 


BEGIN TRY
    DROP TABLE #DatiMeteo
END TRY
BEGIN CATCH
END CATCH"

            End If

            'Dim sql As New StringBuilder

            'sql.Clear()
            'sql.AppendLine("SET NOCOUNT ON ")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @STAZIONE INT = " & CStr(IdStazione))
            'sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
            'sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @MAP_SENSORI TABLE(Id_Sensore INT, Id_TipoSensore INT, FunAggreg VARCHAR(10), ColName VARCHAR(50)) ")
            'sql.AppendLine("INSERT INTO @MAP_SENSORI VALUES ")
            'sql.AppendLine("(NULL, 1, NULL, 'Temp'), ")
            'sql.AppendLine("(NULL, 2, NULL, 'Prec'), ")
            'sql.AppendLine("(NULL, 3, NULL, 'UmRel'), ")
            'sql.AppendLine("(-1, 1, 'min', 'TempMin'), ")
            'sql.AppendLine("(-2, 1, 'max', 'TempMax') ")
            'sql.AppendLine()

            'Select Case TipoSorgente

            '    Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

            '        sql.AppendLine("UPDATE s ")
            '        sql.AppendLine("SET s.Id_Sensore  = s.Id_TipoSensore ")
            '        sql.AppendLine("FROM @MAP_SENSORI s ")
            '        sql.AppendLine("WHERE s.Id_Sensore IS NULL ")

            '    Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

            '        sql.AppendLine("UPDATE s ")
            '        sql.AppendLine("SET s.Id_Sensore = ss.Id_Sensore ")
            '        sql.AppendLine("FROM @MAP_SENSORI s ")
            '        sql.AppendLine("INNER JOIN ( ")
            '        sql.AppendLine("	SELECT Id_Sensore, Id_TipoSensore ")
            '        sql.AppendLine("	FROM MeteoNT_Sensori s WITH (NOLOCK) ")
            '        sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs WITH (NOLOCK) ON sxs.Id_Sensore = s.Id ")
            '        sql.AppendLine("	WHERE sxs.Id_Stazione = @STAZIONE ")
            '        sql.AppendLine(") ss ON ss.Id_TipoSensore = s.Id_TipoSensore AND s.Id_Sensore IS NULL ")

            'End Select

            'sql.AppendLine()
            'sql.AppendLine("UPDATE s ")
            'sql.AppendLine("SET s.FunAggreg = t.FunAggreg ")
            'sql.AppendLine("FROM @MAP_SENSORI s ")
            'sql.AppendLine("INNER JOIN MeteoNT_TipiSensore t ON t.Id = s.Id_TipoSensore ")
            'sql.AppendLine("WHERE s.FunAggreg IS NULL ")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @TMPTABLE TABLE(DataOra DATETIME, Id_Sensore INT, Valore REAL, TipoSensore INT, FunAggreg VARCHAR(50)) ")
            'sql.AppendLine()

            'Select Case TipoSorgente

            '    Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

            '        Dim Tabella_RER As String = "Dati_Meteo_SHH"
            '        Dim ID_Stazione As String = "ID_Stazione"

            '        If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

            '            Tabella_RER = "Dati_Meteo_QHH"
            '            ID_Stazione = "ID_Quadrante"
            '        End If

            '        sql.AppendLine("; WITH TMPDATI_CTE AS ( ")
            '        sql.AppendLine("	SELECT DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, Tempo)), Temp_Media, Precipitazione, UmiditaRelativa ")
            '        sql.AppendLine("	FROM " & Tabella_RER)
            '        sql.AppendLine("	WHERE " & ID_Stazione & " = @STAZIONE And @DATAINIZIO <= Tempo And Tempo <= @DATAFINE ")
            '        sql.AppendLine("), ")
            '        sql.AppendLine("DATI_CTE AS ( ")
            '        sql.AppendLine("	SELECT DataOra, Id_Sensore = 1, Valore = Temp_Media FROM TMPDATI_CTE ")
            '        sql.AppendLine("	UNION ALL ")
            '        sql.AppendLine("	SELECT DataOra, Id_Sensore = 2, Valore = Precipitazione FROM TMPDATI_CTE ")
            '        sql.AppendLine("	UNION ALL ")
            '        sql.AppendLine("	SELECT DataOra, Id_Sensore = 3, Valore = UmiditaRelativa FROM TMPDATI_CTE ")
            '        sql.AppendLine(") ")

            '    Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

            '        sql.AppendLine("; WITH DATI_CTE AS ( ")
            '        sql.AppendLine("	SELECT DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)), d.Id_Sensore, Valore ")
            '        sql.AppendLine("	FROM MeteoNT_DatiOrari d WITH (NOLOCK) ")
            '        sql.AppendLine("	INNER JOIN @MAP_SENSORI s ON s.Id_Sensore = d.Id_Sensore ")
            '        sql.AppendLine("	WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE ")
            '        sql.AppendLine(") ")

            'End Select

            'sql.AppendLine()
            'sql.AppendLine("INSERT INTO @TMPTABLE ")
            'sql.AppendLine("SELECT DataOra, d.Id_Sensore, Valore, Id_TipoSensore, FunAggreg ")
            'sql.AppendLine("FROM DATI_CTE d ")
            'sql.AppendLine("INNER JOIN @MAP_SENSORI s ON s.Id_Sensore = d.Id_Sensore ")
            'sql.AppendLine()
            'sql.AppendLine("UNION ALL ")
            'sql.AppendLine()
            'sql.AppendLine("SELECT DataOra, s2.Id_Sensore, Valore, s2.Id_TipoSensore, s2.FunAggreg ")
            'sql.AppendLine("FROM DATI_CTE d ")
            'sql.AppendLine("INNER JOIN @MAP_SENSORI s1 ON s1.Id_Sensore = d.Id_Sensore ")
            'sql.AppendLine("INNER JOIN @MAP_SENSORI s2 ON s2.Id_TipoSensore = s1.Id_TipoSensore AND S2.Id_Sensore < 0 ")
            'sql.AppendLine()
            'sql.Append(CreateTableDati("DatiMeteo"))

            'sql.AppendLine("INSERT INTO #DatiMeteo ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='avg' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ALL ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='sum' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ALL ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, MIN(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='min' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ALL ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='max' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @SENSORI TABLE(Id_Sensore INT, ColName VARCHAR(MAX)) ")
            'sql.AppendLine("INSERT INTO @SENSORI ")
            'sql.AppendLine("SELECT s.Id_sensore, map.ColName ")
            'sql.AppendLine("FROM (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) s ")
            'sql.AppendLine("INNER JOIN @MAP_SENSORI map ON map.Id_Sensore = s.Id_Sensore ")
            'sql.AppendLine()
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
            'sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
            'sql.AppendLine()
            'sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id_Sensore) FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            'sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + QUOTENAME(Id_Sensore) + ' AS ' + ColName FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            'sql.AppendLine()
            'sql.AppendLine("--Se non esistono Dati @cols è NULL ")
            'sql.AppendLine("IF @cols IS NOT NULL ")
            'sql.AppendLine("BEGIN ")
            'sql.AppendLine()
            'sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
            'sql.AppendLine()
            'sql.AppendLine("END ")
            'sql.AppendLine()
            'sql.Append(DropTableDati("DatiMeteo"))

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

    Public Function LeggiDatiPerIrrigazione(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer,
                                            DataInizio As DateTime, DataFine As DateTime,
                                            ObjParametri_Server As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerIrrigazione"

        Dim DS As New DataSet

        Try

            Dim sql As String = $"SET NOCOUNT ON 

DECLARE @ID_STAZIONE INT = {IdStazione};
DECLARE @STARTPERIOD DATETIME = '{DataInizio.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';
DECLARE @ENDPERIOD DATETIME = '{DataFine.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern)}';"


            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                Dim tbl_dati_meteo As String = "Dati_Meteo_QHH"
                Dim field_id As String = "ID_Quadrante"
                If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then
                    tbl_dati_meteo = "Dati_Meteo_SHH"
                    field_id = "ID_Stazione"
                End If

                sql += $"
SELECT 
	Giorno
	, TMed = ROUND(AVG(Temp_Media), 2)
	, TMin = MIN(Temp_Media)
	, TMax = MAX(Temp_Media)
	, RainMM = ROUND(SUM(Precipitazione), 2)
	, RainH = COUNT(CASE WHEN Precipitazione > 0 THEN 1 END) 
FROM (
	SELECT Giorno = DATEADD(dd, 0, DATEDIFF(dd, 0, Tempo)), Temp_Media, Precipitazione
	FROM {tbl_dati_meteo} WITH (NOLOCK)
	WHERE @STARTPERIOD <= Tempo AND Tempo <= @ENDPERIOD AND {field_id} = @ID_STAZIONE 
) T GROUP BY Giorno ORDER BY Giorno


SELECT col, eti FROM
(
	VALUES
	( 'TMed', 'Temperatura aria' ),
	( 'TMin', 'Temperatura aria' ),
	( 'TMax', 'Temperatura aria' ),
	( 'RaiMM', 'Pluviometro' ),
	( 'RainH', 'Pluviometro' )
) Sens (col, eti)"

            Else

                sql += $"
DECLARE @SENSOR_FLT TABLE(Id_TipoSensore INT, Category INT, [Priority] INT, IsCumulated BIT) 
INSERT INTO @SENSOR_FLT VALUES 
(1, 1, 1, 0),	--Temperatura Aria
(2, 2, 1, 0),	--Pluviometro 
(18, 2, 2, 1),	--Pluviometro cumulato
(15, 3, 0, 0)	--Umidità terreno

DECLARE @AGGR_TBL TABLE (Id_TipoSensoreSrc INT, FunAggreg VARCHAR(50), [Label] VARCHAR(50))
INSERT INTO @AGGR_TBL VALUES
(1, 'avg', 'Tmed'),
(1, 'min', 'Tmin'),
(1, 'max', 'Tmax'),
(2, 'sum', 'RainMM'),
(18, 'sum', 'RainMM'),
(2, 'cnt_gt_0', 'RainH'),
(18, 'cnt_gt_0', 'RainH'),
(15, 'avg', NULL)


DECLARE @SRC_SENSOR TABLE(Id_Sensore INT, Id_TipoSensore INT, IsCumulated BIT) 
INSERT INTO @SRC_SENSOR
SELECT Id_Sensore, Id_TipoSensore, IsCumulated from (
	SELECT 
		Id_Sensore = s.Id 
		, Id_TipoSensore = s.Id_TipoSensore
		, IsCumulated
		, Category
		, [Priority]
		, Flag_Tipo = ROW_NUMBER() OVER (PARTITION BY s.Id_TipoSensore ORDER BY Ordine)
		, Flag_Gategory = ROW_NUMBER() OVER (PARTITION BY Category ORDER BY [Priority])
	FROM MeteoNT_Sensori s WITH (NOLOCK) 
	INNER JOIN MeteoNT_StazioniXSensori x WITH (NOLOCK) ON x.Id_Sensore = s.Id
	INNER JOIN @SENSOR_FLT f ON f.Id_TipoSensore = s.Id_TipoSensore
	WHERE x.Id_Stazione = @ID_STAZIONE
) T WHERE (Flag_Tipo = 1 AND Flag_Gategory = 1) OR [Priority] = 0


DECLARE @SRC_DATI TABLE(Giorno DATETIME, Id_Sensore INT, Id_TipoSensore INT, Valore REAL) 
INSERT INTO @SRC_DATI 
SELECT 
	Giorno = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra))
	, Id_Sensore
	, Id_TipoSensore
	, Valore = IIF(PrecValore IS NULL, Valore, IIF(Valore < PrecValore, Valore, ROUND(Valore - precValore, 2)))
FROM (
	SELECT DataOra, d.Id_Sensore, Id_TipoSensore, Valore, PrecValore = IIF(IsCumulated = 0, NULL, LAG(Valore, 1, NULL) OVER (PARTITION BY d.Id_Sensore ORDER BY d.Id_Sensore, DataOra))
	FROM MeteoNT_DatiOrari d WITH (NOLOCK) 
	INNER JOIN @SRC_SENSOR s ON s.Id_Sensore = d.Id_Sensore 
	WHERE @STARTPERIOD <= DataOra AND DataOra <= @ENDPERIOD
) T


DECLARE @SENSORS TABLE(Id_SensoreSrc INT, Id_SensoreDst INT IDENTITY, [Column] VARCHAR(50), [Label] VARCHAR(MAX), FunAggreg VARCHAR(50))
INSERT INTO @SENSORS
SELECT s.Id, COALESCE(a.[Label], 'UT_' + CAST(s.Id AS VARCHAR(10))), COALESCE(x.Etichetta, s.Etichetta), FunAggreg
FROM @SRC_SENSOR ss
INNER JOIN MeteoNT_Sensori s WITH (NOLOCK) ON s.Id = ss.Id_Sensore
INNER JOIN MeteoNT_StazioniXSensori x WITH (NOLOCK) ON x.Id_Sensore = s.Id AND x.Id_Stazione = @ID_STAZIONE
INNER JOIN @AGGR_TBL a ON a.Id_TipoSensoreSrc = s.Id_TipoSensore


BEGIN TRY
    DROP TABLE #DatiMeteo
END TRY
BEGIN CATCH
END CATCH

CREATE TABLE #DatiMeteo (Giorno DATETIME, Id_Sensore INT, Valore REAL) 

;WITH DATI_CTE AS (
	SELECT Giorno, Id_Sensore = Id_SensoreDst, Valore, FunAggreg
	FROM @SRC_DATI d
	INNER JOIN @SENSORS a ON a.Id_SensoreSrc = d.Id_Sensore
)

INSERT INTO #DatiMeteo
SELECT Giorno, Id_Sensore, Valore = AVG(Valore) FROM DATI_CTE WHERE FunAggreg = 'avg' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = MIN(Valore) FROM DATI_CTE WHERE FunAggreg = 'min' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = MAX(Valore) FROM DATI_CTE WHERE FunAggreg = 'max' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = SUM(Valore) FROM DATI_CTE WHERE FunAggreg = 'sum' GROUP BY Giorno, Id_Sensore
UNION ALL
SELECT Giorno, Id_Sensore, Valore = COUNT(CASE WHEN Valore > 0 THEN 1 END) FROM DATI_CTE WHERE FunAggreg = 'cnt_gt_0' GROUP BY Giorno, Id_Sensore


DELETE FROM @SENSORS WHERE Id_SensoreDst NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo)

DECLARE @columns VARCHAR(MAX), @pivot VARCHAR(MAX)

SELECT
	@columns = COALESCE(@columns + ', ', '') + QUOTENAME(TRIM([Column])) + ' = ' + QUOTENAME(TRIM(CAST(Id_SensoreDst AS VARCHAR(5))))
	, @pivot = COALESCE(@pivot + ', ', '') + QUOTENAME(TRIM(CAST(Id_SensoreDst AS VARCHAR(5))))
FROM @SENSORS

IF @columns IS NOT NULL 
BEGIN 

    EXECUTE('SELECT Giorno AS [Date], ' + @columns + ' FROM (SELECT Giorno, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @pivot + ')) p ORDER BY Giorno') 

	SELECT col = [Column], eti = [Label] FROM @SENSORS
END 


BEGIN TRY
    DROP TABLE #DatiMeteo
END TRY
BEGIN CATCH
END CATCH"

            End If

            'Dim sql As New StringBuilder

            'sql.Clear()

            'sql.AppendLine("SET NOCOUNT ON ")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @STAZIONE INT = " & CStr(IdStazione))
            'sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
            'sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @TBL_ALIAS TABLE(Id INT, Id_TipoSensoreSrc INT, Alias VARCHAR(50), FunAggreg VARCHAR(50)) ")
            'sql.AppendLine("INSERT INTO @TBL_ALIAS VALUES ")
            'sql.AppendLine("(NULL, 1, 'Tmed', NULL), ")
            'sql.AppendLine("(NULL, 2, 'RainMM', NULL), ")
            'sql.AppendLine("(NULL, 15, NULL, NULL), ")
            'sql.AppendLine("(-1, 1, 'Tmin', 'min'), ")
            'sql.AppendLine("(-2, 1, 'Tmax', 'max'), ")
            'sql.AppendLine("(-3, 2, 'RainH', 'cnt_gt_0') ")
            'sql.AppendLine()
            'sql.AppendLine("UPDATE @TBL_ALIAS SET FunAggreg = (SELECT FunAggreg FROM MeteoNT_TipiSensore WHERE Id = Id_TipoSensoreSrc) ")
            'sql.AppendLine("WHERE FunAggreg IS NULL ")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @TMP_Sensori TABLE(Id INT, Id_TipoSensore INT, Etichetta VARCHAR(MAX)) ")

            'Select Case TipoSorgente

            '    Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

            '        sql.AppendLine("INSERT INTO @TMP_Sensori ")
            '        sql.AppendLine("SELECT Id, Id_TipoSensore, Etichetta = COALESCE(sxs.Etichetta, sens.Etichetta) ")
            '        sql.AppendLine("FROM MeteoNT_Sensori sens WITH (NOLOCK)")
            '        sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori sxs WITH (NOLOCK) ON sxs.Id_Sensore = sens.Id ")
            '        sql.AppendLine("WHERE sxs.Id_Stazione = @STAZIONE ")

            '    Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

            '        sql.AppendLine("INSERT INTO @TMP_Sensori VALUES ")
            '        sql.AppendLine("(1, 1, 'Temperatura aria'), ")
            '        sql.AppendLine("--(2, 3, 'Umidità relativa'), ")
            '        sql.AppendLine("--(3, 4, 'Bagnatura fogliare'), ")
            '        sql.AppendLine("(4, 2, 'Pioggia') ")

            'End Select

            'sql.AppendLine()
            'sql.AppendLine("DECLARE @TBL_SENSORI TABLE(Id INT, Id_Src INT, Etichetta VARCHAR(MAX), FunAggreg VARCHAR(10)) ")
            'sql.AppendLine("INSERT INTO @TBL_SENSORI ")
            'sql.AppendLine("SELECT ")
            'sql.AppendLine("	COALESCE(alias.Id, sens.Id) AS Id ")
            'sql.AppendLine("	, sens.Id AS Id_Src ")
            'sql.AppendLine("	, COALESCE(alias.Alias, 'UT_' + CAST(sens.Id AS VARCHAR(10))) AS Etichetta --sens.Etichetta) AS Etichetta ")
            'sql.AppendLine("	, alias.FunAggreg ")
            'sql.AppendLine("FROM @TMP_Sensori sens ")
            'sql.AppendLine("INNER JOIN @TBL_ALIAS alias on alias.Id_TipoSensoreSrc = sens.Id_TipoSensore ")
            'sql.AppendLine()

            'Dim tblDati As String = "MeteoNT_DatiOrari"

            'If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

            '    Dim Tabella_RER As String = "Dati_Meteo_SHH"
            '    Dim WhereClause As String = "ID_Stazione"

            '    If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then
            '        Tabella_RER = "Dati_Meteo_QHH"
            '        WhereClause = "ID_Quadrante"
            '    End If

            '    WhereClause = "WHERE " & WhereClause & " = @STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE "

            '    sql.AppendLine("DECLARE @TMP_DATI TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL); ")
            '    sql.AppendLine("WITH DATI_CTE AS ( ")
            '    sql.AppendLine("    SELECT Tempo, Temp_Media, Precipitazione FROM " & Tabella_RER & " " & WhereClause)
            '    sql.AppendLine(") ")
            '    sql.AppendLine("INSERT INTO @TMP_DATI ")
            '    sql.AppendLine("SELECT 1, Tempo, Temp_Media FROM DATI_CTE ")
            '    sql.AppendLine("--UNION ")
            '    sql.AppendLine("--SELECT 2, Tempo, UmiditaRelativa FROM DATI_CTE ")
            '    sql.AppendLine("--UNION ")
            '    sql.AppendLine("--SELECT 3, Tempo, Bagnatura FROM DATI_CTE ")
            '    sql.AppendLine("UNION ")
            '    sql.AppendLine("SELECT 4, Tempo, Precipitazione FROM DATI_CTE ")
            '    sql.AppendLine()

            '    tblDati = "@TMP_DATI"
            'End If

            'sql.AppendLine("DECLARE @TBL_DATI TABLE(DataOra DATETIME, Id_Sensore INT, Valore REAL, FunAggreg VARCHAR(50)) ")
            'sql.AppendLine("INSERT INTO @TBL_DATI ")
            'sql.AppendLine("SELECT ")
            'sql.AppendLine("	DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)) ")
            'sql.AppendLine("    , Id_Sensore = sens.Id ")
            'sql.AppendLine("    , Valore ")
            'sql.AppendLine("    , FunAggreg ")
            'sql.AppendLine("FROM " & tblDati & " dati WITH (NOLOCK) ")
            'sql.AppendLine("INNER JOIN @TBL_SENSORI sens ON sens.Id_Src = dati.Id_Sensore ")

            'If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali OrElse
            '    TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner OrElse
            '    TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

            '    sql.AppendLine("WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE ")
            'End If

            'sql.AppendLine()
            'sql.Append(CreateTableDati("DatiMeteo"))

            'sql.AppendLine("INSERT INTO #DatiMeteo ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='avg' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, MIN(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='min' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='max' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='sum' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine("UNION ")
            'sql.AppendLine("SELECT DataOra, Id_Sensore, COUNT(CASE WHEN Valore > 0 THEN 1 END) AS Valore FROM @TBL_DATI WHERE FunAggreg='cnt_gt_0' GROUP BY DataOra, Id_Sensore ")
            'sql.AppendLine()
            'sql.AppendLine("DELETE FROM @TBL_SENSORI WHERE Id NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) ")
            'sql.AppendLine()
            'sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
            'sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
            'sql.AppendLine()
            'sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id) FROM @TBL_SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            'sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + QUOTENAME(Id) + ' AS ' + QUOTENAME(Etichetta) FROM @TBL_SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            'sql.AppendLine()
            'sql.AppendLine("--Se non esistono Dati @cols è NULL ")
            'sql.AppendLine("IF @cols IS NOT NULL ")
            'sql.AppendLine("BEGIN ")
            'sql.AppendLine()
            'sql.AppendLine("    EXECUTE('SELECT DataOra As Date, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
            'sql.AppendLine()
            'sql.AppendLine("	SELECT col = s1.Etichetta, eti = s2.Etichetta ")
            'sql.AppendLine("	FROM @TBL_SENSORI s1 ")
            'sql.AppendLine("	INNER JOIN MeteoNT_Sensori s2 on s2.Id = s1.Id_Src ")
            'sql.AppendLine()
            'sql.AppendLine("END ")
            'sql.AppendLine()
            'sql.Append(DropTableDati("DatiMeteo"))

            EseguiQuery_Lettura(ObjParametri_Server, sql, NomeRoutine, DS, "DatiMeteo_NT")

            If DS.Tables.Count < 2 Then

                DS = Nothing
            End If

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DS
    End Function

#End Region



    Public Class Upsert_Helper
        Private ReadOnly sql As StringBuilder
        Private comma As String
        Private counter As Integer

        Public ReadOnly Property QuerySql As String
            Get
                Return sql.ToString()
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return counter
            End Get
        End Property

        Public Sub New()
            sql = New StringBuilder
            comma = ""
            counter = 0
        End Sub
        Public Sub Add(ByVal ParamArray args() As String)
            If args.Length <= 0 Then
                Return
            End If

            Dim str As String = ""
            Dim c As String = ""
            For i As Integer = 0 To UBound(args, 1)
                str &= c & args(i)
                c = ", "
            Next

            sql.AppendLine(comma & "(" & str & ")")
            comma = ", "
            counter += 1
        End Sub
        Public Function GetAndReset() As String

            Dim retstr As String = sql.ToString()

            sql.Clear()
            comma = ""
            counter = 0

            Return retstr
        End Function
    End Class



#Region "Gestione WiNet"

    Public Function Upsert_WinetReti(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_WinetReti"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Rete INT, Nome VARCHAR(MAX), Descrizione VARCHAR(MAX)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Rete, Nome, Descrizione) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO WiNet2_Reti AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Rete = s.Id_Rete ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Nome = s.Nome, Descrizione = s.Descrizione ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Rete, Nome, Descrizione, FlagScaricoDati) ")
                sql.AppendLine("		VALUES (s.Id_Rete, s.Nome, s.Descrizione, 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_WinetNodi(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_WinetNodi"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Rete INT, Id_Nodo INT, Etichetta VARCHAR(MAX), Lat FLOAT, Lng FLOAT, IsCoordinator BIT) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Rete, Id_Nodo, Etichetta, Lat, Lng, IsCoordinator) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO WiNet2_Nodi AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Rete = s.Id_Rete AND t.Id_Nodo = s.Id_Nodo ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Etichetta = s.Etichetta, Lat = s.Lat, Lng = s.Lng, IsCoordinator = s.IsCoordinator ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Rete, Id_Nodo, Etichetta, Lat, Lng, IsCoordinator) ")
                sql.AppendLine("		VALUES (s.Id_Rete, s.Id_Nodo, s.Etichetta, s.Lat, s.Lng, s.IsCoordinator) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_WinetSensori(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_WinetSensori"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Rete INT, Id_Nodo INT, Id_Sensore INT, Etichetta VARCHAR(MAX), Tipo VARCHAR(MAX), UM VARCHAR(MAX)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Rete, Id_Nodo, Id_Sensore, Etichetta, Tipo, UM) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO WiNet2_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Rete = s.Id_Rete AND t.Id_Nodo = s.Id_Nodo AND t.Id_Sensore = s.Id_Sensore ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Etichetta = s.Etichetta, Tipo = s.Tipo, UM = s.UM ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Rete, Id_Nodo, Id_Sensore, Etichetta, Tipo, UM) ")
                sql.AppendLine("		VALUES (s.Id_Rete, s.Id_Nodo, s.Id_Sensore, s.Etichetta, s.Tipo, s.UM) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next


            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_WinetDati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_WinetDati"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Rete INT, Id_Nodo INT, Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC, Valore FROM ( ")
                sql.AppendLine("	SELECT Id, DataOraUTC, Valore, NRow = ROW_NUMBER() OVER(PARTITION BY Id, DataOraUTC ORDER BY Id, DataOraUTC) ")
                sql.AppendLine("	FROM @TMPTABLE tt ")
                sql.AppendLine("	INNER JOIN WiNet2_Sensori s ON s.Id_Rete = tt.Id_Rete AND s.Id_Nodo = tt.Id_Nodo AND s.Id_Sensore = tt.Id_Sensore ")
                sql.AppendLine(") T WHERE NRow = 1 ")
                'sql.AppendLine("SELECT Id, DataOraUTC, Valore ")
                'sql.AppendLine("FROM @TMPTABLE tt ")
                'sql.AppendLine("INNER JOIN WiNet2_Sensori s ON s.Id_Rete = tt.Id_Rete AND s.Id_Nodo = tt.Id_Nodo AND s.Id_Sensore = tt.Id_Sensore ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO WiNet2_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoWiNet(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoWiNet"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = Id_Rete, UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT sens.Id_Rete, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	FROM Winet2_Sensori sens ")
            sql.AppendLine("    INNER JOIN WiNet2_Reti staz ON staz.Id_Rete = sens.Id_Rete AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("    LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM Winet2_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine(") T GROUP BY Id_Rete ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione AS"

    Public Function Upsert_AS_Sistemi(ByVal sqlValues As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_AS_Sistemi"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim nInsert As Integer = 0

        Try
            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Sistema INT) ")
            sql.AppendLine("INSERT INTO @TMPTABLE (Id_Sistema) VALUES ")
            sql.Append(sqlValues)
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO AS_Sistemi AS t ")
            sql.AppendLine("    USING ")
            sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
            sql.AppendLine("    ON t.Id_Sistema = s.Id_Sistema ")
            sql.AppendLine("	--WHEN MATCHED THEN ")
            sql.AppendLine("	--	UPDATE SET Nome = ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Id_Sistema, Nome, FlagScaricoDati) ")
            sql.AppendLine("		VALUES (s.Id_Sistema, '', 1) ")
            'per AS Scarico comunque i dati della stazione appena inserita, altrimenti avrei da riprocessare i files XML
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            nInsert = 0
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return nInsert

    End Function

    Public Function Upsert_AS_Unita(ByVal sqlValues As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_AS_Unita"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim nInsert As Integer = 0

        Try
            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Sistema INT, Id_Unita INT) ")
            sql.AppendLine("INSERT INTO @TMPTABLE (Id_Sistema, Id_Unita) VALUES ")
            sql.Append(sqlValues)
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO AS_Unita AS t ")
            sql.AppendLine("    USING ")
            sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
            sql.AppendLine("    ON t.Id_Sistema = s.Id_Sistema AND t.Id_Unita = s.Id_Unita ")
            sql.AppendLine("	--WHEN MATCHED THEN ")
            sql.AppendLine("	--	UPDATE SET Nome = ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Id_Sistema, Id_Unita, Nome) ")
            sql.AppendLine("		VALUES (s.Id_Sistema, s.Id_Unita, '') ")
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            nInsert = 0
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return nInsert

    End Function

    Public Function Upsert_AS_Sensori(ByVal sqlValues As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_AS_Sensori"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim nInsert As Integer = 0

        Try
            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Sistema INT, Id_Unita INT, Id_Sensore INT, UM VARCHAR(MAX)) ")
            sql.AppendLine("INSERT INTO @TMPTABLE (Id_Sistema, Id_Unita, Id_Sensore, UM) VALUES ")
            sql.Append(sqlValues)
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO AS_Sensori AS t ")
            sql.AppendLine("    USING ")
            sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
            sql.AppendLine("    ON t.Id_Sistema = s.Id_Sistema AND t.Id_Unita = s.Id_Unita AND t.Id_Sensore = s.Id_Sensore ")
            sql.AppendLine("	--WHEN MATCHED THEN ")
            sql.AppendLine("	--	UPDATE SET UM = s.UM ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Id_Sistema, Id_Unita, Id_Sensore, UM) ")
            sql.AppendLine("		VALUES (s.Id_Sistema, s.Id_Unita, s.Id_Sensore, s.UM) ")
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            nInsert = 0
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return nInsert

    End Function

    Public Function Upsert_AS_Dati(ByVal sqlValues As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_AS_Dati"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim nInsert As Integer = 0

        Try
            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Sistema INT, Id_Unita INT, Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
            sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
            sql.Append(sqlValues)
            sql.AppendLine()
            sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
            sql.AppendLine("INSERT INTO @SRCTABLE ")
            sql.AppendLine("SELECT Id, DataOraUTC, Valore ")
            sql.AppendLine("FROM @TMPTABLE tt ")
            sql.AppendLine("INNER JOIN AS_Sensori s ON s.Id_Sistema = tt.Id_Sistema AND s.Id_Unita = tt.Id_Unita AND s.Id_Sensore = tt.Id_Sensore ")
            sql.AppendLine("INNER JOIN AS_Sistemi sis ON sis.Id_Sistema = s.Id_Sistema ")
            sql.AppendLine("WHERE sis.FlagScaricoDati = 1 ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO AS_Dati AS t ")
            sql.AppendLine("	USING ")
            sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
            sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
            sql.AppendLine("	WHEN MATCHED THEN ")
            sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
            sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            nInsert = 0
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return nInsert

    End Function

#End Region


#Region "Gestione NetSens"

    Public Function Upsert_NetSensStazioni(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_NetSensStazioni"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, Nome VARCHAR(MAX)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Stazione, Nome) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO NetSens_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Stazione = s.Id_Stazione ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Nome = s.Nome ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Stazione, Nome, FlagScaricoDati) ")
                sql.AppendLine("		VALUES (s.Id_Stazione, s.Nome, 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_NetSensUnita(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_NetSensUnita"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, Id_Unita INT, Nome VARCHAR(MAX)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Stazione, Id_Unita, Nome) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO NetSens_Unita AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Stazione = s.Id_Stazione AND t.Id_Unita = s.Id_Unita ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Nome = s.Nome ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Stazione, Id_Unita, Nome) ")
                sql.AppendLine("		VALUES (s.Id_Stazione, s.Id_Unita, s.Nome) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_NetSensSensori(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_NetSensSensori"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, Id_Unita INT, Id_Sensore INT, Nome VARCHAR(MAX), UM VARCHAR(MAX)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Stazione, Id_Unita, Id_Sensore, Nome, UM) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO NetSens_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Stazione = s.Id_Stazione AND t.Id_Unita = s.Id_Unita AND t.Id_Sensore = s.Id_Sensore ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Nome = s.Nome, UM = s.UM ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Stazione, Id_Unita, Id_Sensore, Nome, UM) ")
                sql.AppendLine("		VALUES (s.Id_Stazione, s.Id_Unita, s.Id_Sensore, s.Nome, s.UM) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_NetSensDati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_NetSensDati"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()
                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, Id_Unita INT, Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC, Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN NetSens_Sensori s ON s.Id_Stazione = tt.Id_Stazione AND s.Id_Unita = tt.Id_Unita AND s.Id_Sensore = tt.Id_Sensore ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO NetSens_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If
        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoNetSens(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoNetSens"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione, UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT sens.Id_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	FROM NetSens_Sensori sens ")
            sql.AppendLine("    INNER JOIN NetSens_Stazioni staz ON staz.Id_Stazione = sens.Id_Stazione AND staz.FlagScaricoDati = 1")
            sql.AppendLine("    LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM NetSens_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine(") T GROUP BY Id_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Pessl"

    Public Function Upsert_Pessl_Stations(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Pessl_Stations"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id] CHAR(10), [Name] VARCHAR(MAX), [Lat] FLOAT, [Lng] FLOAT, [Alt] FLOAT, [TimezoneOffset] INT, [MisInterval] INT, [LogInterval] INT, [MinDate] DATETIME) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id], [Name], [Lat], [Lng], [Alt], [TimezoneOffset], [MisInterval], [LogInterval], [MinDate]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Pessl_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id] = s.[Id] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id], [Name], [Lat], [Lng], [Alt], [TimezoneOffset], [MisInterval], [LogInterval], [MinDate], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Id], s.[Name], s.[Lat], s.[Lng], s.[Alt], s.[TimezoneOffset], s.[MisInterval], s.[LogInterval], s.[MinDate], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Pessl_Sensors(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Pessl_Sensors"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Station] CHAR(10), [Name] VARCHAR(50), [Unit] VARCHAR(20), [Code] INT, [Ch] INT, [Mac] VARCHAR(20), [Serial] VARCHAR(20), [Aggr] VARCHAR(20)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Station], [Name], [Unit], [Code], [Ch], [Mac], [Serial], [Aggr]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Pessl_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Station] = s.[Id_Station] AND t.[Code] = s.[Code] AND t.[Ch] = s.[Ch] AND t.[Mac] = s.[Mac] AND t.[Serial] = s.[Serial] AND t.[Aggr] = s.[Aggr] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Station], [Name], [Unit], [Code], [Ch], [Mac], [Serial], [Aggr]) ")
                sql.AppendLine("		VALUES (s.[Id_Station], s.[Name], s.[Unit], s.[Code], s.[Ch], s.[Mac], s.[Serial], s.[Aggr]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Pessl_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Pessl_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Station CHAR(10), Pessl_Sensor CHAR(100), DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOra, Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , Id_Station ")
                sql.AppendLine("        , CAST(Ch AS VARCHAR(20)) + '_' + Mac +'_' + Serial + '_' + CAST(Code AS VARCHAR(20)) + '_' + Aggr AS Pessl_Sensor ")
                '>>>>>>>>>>>>>>>>>>>>>>>  CH_MAC_SERIAL_CODE_AGGR
                'Potrebbe anche essere:   CH_SERIAL_MAC_CODE_AGGR
                'sql.AppendLine("        , CAST(Ch AS VARCHAR(20)) + '_' + Serial +'_' + Mac + '_' + CAST(Code AS VARCHAR(20)) + '_' + Aggr AS Pessl_Sensor ")
                sql.AppendLine("    FROM Pessl_Sensori ")
                sql.AppendLine(") s ON s.Id_Station = tt.Id_Station AND s.Pessl_Sensor = tt.Pessl_Sensor ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Pessl_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOra, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoPessl(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoPessl"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = TRIM(Id_Station), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT sens.Id_Station, UltimaDataOra = COALESCE(dati.UltimaDataOra, staz.MinDate, @DEF_DATA) ")
            sql.AppendLine("	FROM Pessl_Sensori sens ")
            sql.AppendLine("    INNER JOIN Pessl_Stazioni staz ON staz.Id = sens.Id_Station AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("    LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM Pessl_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine(") T GROUP BY Id_Station ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione GreenPlanet"

    Public Function Upsert_GreenPlanet_Stazioni(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_GreenPlanet_Stazioni"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Stazione] INT, [Name] VARCHAR(MAX), [City] VARCHAR(MAX), [Location] VARCHAR(MAX), [Utmx] FLOAT, [Utmy] FLOAT, [Elevation] VARCHAR(25)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Stazione], [Name], [City], [Location], [Utmx], [Utmy], [Elevation]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO GreenPlanet_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Stazione] = s.[Id_Stazione] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Stazione], [Name], [City], [Location], [Utmx], [Utmy], [Elevation], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Id_Stazione], s.[Name], s.[City], s.[Location], s.[Utmx], s.[Utmy], s.[Elevation], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_GreenPlanet_Sensori(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_GreenPlanet_Sensori"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Stazione] INT, [Sensore] VARCHAR(50)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Stazione], [Sensore]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO GreenPlanet_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Stazione] = s.[Id_Stazione] AND t.[Sensore] = s.[Sensore] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Stazione], [Sensore]) ")
                sql.AppendLine("		VALUES (s.[Id_Stazione], s.[Sensore]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_GreenPlanet_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_GreenPlanet_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, Sensore VARCHAR(50), DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOra, Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , Id_Stazione ")
                sql.AppendLine("        , Sensore ")
                sql.AppendLine("    FROM GreenPlanet_Sensori ")
                sql.AppendLine(") s ON s.Id_Stazione = tt.Id_Stazione AND s.Sensore = tt.Sensore ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO GreenPlanet_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOra, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoGreenPlanet(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoGreenPlanet"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione, UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT staz.Id_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	FROM GreenPlanet_Stazioni staz ")
            sql.AppendLine("    LEFT JOIN GreenPlanet_Sensori sens ON sens.Id_Stazione = staz.Id_Stazione ")
            sql.AppendLine("    LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM GreenPlanet_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	WHERE staz.FlagScaricoDati = 1 ")
            sql.AppendLine(") T GROUP BY Id_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Acmotec"
    Public Function Upsert_Acmotec_Devices(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Acmotec_Devices"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([guid] VARCHAR(32),type VARCHAR(30), [name] VARCHAR(250), [serial] VARCHAR(50), [Lat] FLOAT, [Lng] FLOAT, [CreatedAt] DATETIME)")
                sql.AppendLine("INSERT INTO @TMPTABLE ([guid], [type], [name], [serial], [Lat], [Lng], [CreatedAt]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[guid] = s.[guid] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([guid], [type], [name], [serial], [Lat], [Lng], [CreatedAt], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[guid], s.[type], s.[name], s.[serial], s.[Lat], s.[Lng], s.[CreatedAt], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Acmotec_Quantities(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Acmotec_Quantities"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Stazione] varchar(32),[MeasureId] varchar(50), [MeasureName] varchar(MAX),  [MeasureUnit] varchar(50), [CreatedAt] DATETIME) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Stazione], [MeasureId], [MeasureName], [MeasureUnit], [CreatedAt] ) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Stazione] = s.[Id_Stazione] AND t.[MeasureName] = s.[MeasureName] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Stazione], [MeasureId], [MeasureName], [MeasureUnit], [CreatedAt] ) ")
                sql.AppendLine("		VALUES (s.[Id_Stazione], s.[MeasureId], s.[MeasureName], s.[MeasureUnit], s.[CreatedAt]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Acmotec_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_A2A_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione CHAR(32), MeasureId CHAR(50), DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC, Valore FROM (")
                sql.AppendLine("    SELECT Id, DataOraUTC, Valore, NRow = ROW_NUMBER() OVER(PARTITION BY Id, DataOraUTC ORDER BY Id, DataOraUTC) ")
                sql.AppendLine("	FROM @TMPTABLE tt ")
                sql.AppendLine("    INNER JOIN Acmotec_Sensori s ON s.Id_Stazione = tt.Id_Stazione AND s.MeasureId = tt.MeasureId ")
                sql.AppendLine(") T WHERE NRow = 1")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoAcmotec(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoA2A"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = TRIM(Id_Stazione), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Id_Stazione, UltimaDataOra = CASE WHEN StazioneDataOra > UltimaDataOra THEN StazioneDataOra ELSE UltimaDataOra END FROM ( ")
            sql.AppendLine("	    SELECT sens.Id_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA), StazioneDataOra = COALESCE(staz.CreatedAt, @DEF_DATA) ")
            sql.AppendLine("	    FROM Acmotec_Sensori sens ")
            sql.AppendLine("	    INNER JOIN Acmotec_Stazioni staz ON staz.guid = sens.Id_Stazione AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM Acmotec_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("    ) T")
            sql.AppendLine(") T GROUP BY Id_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function
#End Region


#Region "Gestione A2ASmartCity"

    Public Function Upsert_A2A_Devices(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_A2A_Devices"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id] CHAR(40), [Name] VARCHAR(MAX), [IdType] INT, [Type] VARCHAR(MAX), [IdModel] INT, [Model] VARCHAR(MAX), [IdGroup] INT, [Group] VARCHAR(MAX), [IdBrand] INT, [Brand] VARCHAR(MAX), [Lat] FLOAT, [Lng] FLOAT, [Address] VARCHAR(MAX), [CreatedAt] DATETIME)")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id], [Name], [IdType], [Type], [IdModel], [Model], [IdGroup], [Group], [IdBrand], [Brand], [Lat], [Lng], [Address], [CreatedAt]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO A2A_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id] = s.[Id] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id], [Name], [IdType], [Type], [IdModel], [Model], [IdGroup], [Group], [IdBrand], [Brand], [Lat], [Lng], [Address], [CreatedAt], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Id], s.[Name], s.[IdType], s.[Type], s.[IdModel], s.[Model], s.[IdGroup], s.[Group], s.[IdBrand], s.[Brand], s.[Lat], s.[Lng], s.[Address], s.[CreatedAt], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_A2A_Quantities(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_A2A_Quantities"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Stazione] char(40), [MeasureName] varchar(40), [MeasureId] varchar(20), [UnitOfMeasure] varchar(10)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Stazione], [MeasureName], [MeasureId], [UnitOfMeasure]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO A2A_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Stazione] = s.[Id_Stazione] AND t.[MeasureName] = s.[MeasureName] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Stazione], [MeasureName], [MeasureId], [UnitOfMeasure]) ")
                sql.AppendLine("		VALUES (s.[Id_Stazione], s.[MeasureName], s.[MeasureId], s.[UnitOfMeasure]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_A2A_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_A2A_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione CHAR(40), MeasureName CHAR(40), DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC, Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , Id_Stazione ")
                sql.AppendLine("        , MeasureName ")
                sql.AppendLine("    FROM A2A_Sensori ")
                sql.AppendLine(") s ON s.Id_Stazione = tt.Id_Stazione AND s.MeasureName = tt.MeasureName ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO A2A_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoA2A(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoA2A"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = TRIM(Id_Stazione), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Id_Stazione, UltimaDataOra = CASE WHEN StazioneDataOra > UltimaDataOra THEN StazioneDataOra ELSE UltimaDataOra END FROM ( ")
            sql.AppendLine("	    SELECT sens.Id_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA), StazioneDataOra = COALESCE(staz.CreatedAt, @DEF_DATA) ")
            sql.AppendLine("	    FROM A2A_Sensori sens ")
            sql.AppendLine("	    INNER JOIN A2A_Stazioni staz ON staz.Id = sens.Id_Stazione AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM A2A_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("    ) T")
            sql.AppendLine(") T GROUP BY Id_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione DigiFarm"

    Public Function Upsert_DigiFarm_Stations(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_DigiFarm_Stations"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id] CHAR(40), [Name] VARCHAR(MAX), [Description] VARCHAR(MAX), [InstallationDate] DATETIME, [Type] VARCHAR(MAX), [Lat] FLOAT, [Lng] FLOAT)")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id], [Name], [Description], [InstallationDate], [Type], [Lat], [Lng]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO DigiFarm_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id] = s.[Id] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id], [Name], [Description], [InstallationDate], [Type], [Lat], [Lng], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Id], s.[Name], s.[Description], s.[InstallationDate], s.[Type], s.[Lat], s.[Lng], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_DigiFarm_Sensors(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_DigiFarm_Sensors"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Stazione] char(40), [SensorId] varchar(20), [SensorLabel] varchar(MAX), [SensorType] varchar(MAX), [SensorUM] varchar(10)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Stazione], [SensorId], [SensorLabel], [SensorType], [SensorUM]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO DigiFarm_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Stazione] = s.[Id_Stazione] AND t.[SensorId] = s.[SensorId] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Stazione], [SensorId], [SensorLabel], [SensorType], [SensorUM]) ")
                sql.AppendLine("		VALUES (s.[Id_Stazione], s.[SensorId], s.[SensorLabel], s.[SensorType], s.[SensorUM]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_DigiFarm_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_DigiFarm_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione CHAR(40), SensorId CHAR(20), DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOra, Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , Id_Stazione ")
                sql.AppendLine("        , SensorId ")
                sql.AppendLine("    FROM DigiFarm_Sensori ")
                sql.AppendLine(") s ON s.Id_Stazione = tt.Id_Stazione AND s.SensorId = tt.SensorId ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO DigiFarm_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOra, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoDigiFarm(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoDigiFarm"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = TRIM(Id_Stazione), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Id_Stazione, UltimaDataOra = CASE WHEN StazioneDataOra > UltimaDataOra THEN StazioneDataOra ELSE UltimaDataOra END FROM ( ")
            sql.AppendLine("    	SELECT sens.Id_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA), StazioneDataOra = COALESCE(staz.InstallationDate, @DEF_DATA) ")
            sql.AppendLine("	    FROM DigiFarm_Sensori sens ")
            sql.AppendLine("	    INNER JOIN DigiFarm_Stazioni staz ON staz.Id = sens.Id_Stazione AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM DigiFarm_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	) T ")
            sql.AppendLine(") T GROUP BY Id_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione CoDiMa"

    Public Function CoDiMa_Stazioni_UltimaModifica(ByVal ObjParametri_Server As AgronicaCoreParametri) As DateTime

        Dim UltimaModifica As DateTime = New Date(1900, 1, 1)

        Try

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server,
                                                      "SELECT UltimaModifica = COALESCE(MAX(DataUltimaModifica), CAST('1900-01-01' AS DATETIME)) FROM CoDiMa2_Stazioni",
                                                      "MeteoNT.CoDiMa_Stazioni_UltimaModifica")

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                UltimaModifica = CDate(DT.Rows(0)("UltimaModifica"))
            End If
        Catch ex As Exception

            UltimaModifica = New Date(1900, 1, 1)
        End Try

        Return UltimaModifica
    End Function

    Public Function Upsert_CoDiMa_Stazioni(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_CoDiMa_Stazioni"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Codice] VARCHAR(10), [Nome] VARCHAR(MAX), [Sensori] VARCHAR(MAX), [Boaga_x] FLOAT, [Boaga_y] FLOAT, [Lat] FLOAT, [Lng] FLOAT)")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Codice], [Nome], [Sensori], [Boaga_x], [Boaga_y], [Lat], [Lng]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO CoDiMa2_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Codice] = s.[Codice] ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET t.DataUltimaModifica = GETDATE() ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Codice], [Nome], [Sensori], [Boaga_x], [Boaga_y], [Lat], [Lng], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Codice], s.[Nome], s.[Sensori], s.[Boaga_x], s.[Boaga_y], s.[Lat], s.[Lng], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_CoDiMa_Sensori(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_CoDiMa_Sensori"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Codice_Stazione] varchar(10), [Sensore] varchar(50), [Etichetta] varchar(max)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Codice_Stazione], [Sensore], [Etichetta]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO CoDiMa2_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Codice_Stazione] = s.[Codice_Stazione] AND t.[Sensore] = s.[Sensore] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Codice_Stazione], [Sensore], [Etichetta]) ")
                sql.AppendLine("		VALUES (s.[Codice_Stazione], s.[Sensore], s.[Etichetta]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_CoDiMa_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_CoDiMa_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL, IsUpdate BIT DEFAULT(0)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE (Id_Sensore, DataOra, Valore) ")
                sql.AppendLine("SELECT Id_Sensore = s.Id, T.DataOra, T.Valore ")
                sql.AppendLine("FROM CoDiMa2_Sensori s ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("	SELECT Codice_Stazione = TRIM(CAST(Codice_Stazione AS VARCHAR(10))), Sensore = TRIM(CAST(Sensore AS VARCHAR(50))), DataOra = CAST(DataOra AS DATETIME), Valore = CAST(Valore AS REAL) ")
                sql.AppendLine("	FROM (VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine("	) SRC(Codice_Stazione, Sensore, DataOra, Valore) ")
                sql.AppendLine(") T ON T.Codice_Stazione = s.Codice_Stazione AND T.Sensore = s.Sensore ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ImportRows INT = @@ROWCOUNT; ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @Updated TABLE (Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL PRIMARY KEY CLUSTERED (Id_Sensore, DataOra)); ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @RESULT_UPDATE INT = 0 ")
                sql.AppendLine("DECLARE @RESULT_INSERT INT = 0 ")
                sql.AppendLine()
                sql.AppendLine("BEGIN TRY ")
                sql.AppendLine()
                sql.AppendLine("	BEGIN TRAN; ")
                sql.AppendLine()
                sql.AppendLine("	UPDATE t ")
                sql.AppendLine("	SET t.Valore = tt.Valore, Elaborato = 0 ")
                sql.AppendLine("	OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO @Updated (Id_Sensore, DataOra) ")
                sql.AppendLine("	FROM CoDiMa2_Dati t ")
                sql.AppendLine("	INNER JOIN @TMPTABLE tt ON tt.Id_Sensore = t.Id_Sensore AND tt.DataOra = t.DataOra ")
                sql.AppendLine()
                sql.AppendLine("	SET @RESULT_UPDATE = @@ROWCOUNT ")
                sql.AppendLine()
                sql.AppendLine("	IF (@RESULT_UPDATE < @ImportRows) -- if all rows were updates then skip, else insert remaining ")
                sql.AppendLine("	BEGIN ")
                sql.AppendLine()
                sql.AppendLine("		UPDATE s ")
                sql.AppendLine("		SET s.IsUpdate = 1 ")
                sql.AppendLine("		FROM @TMPTABLE s ")
                sql.AppendLine("		INNER JOIN @Updated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
                sql.AppendLine()
                sql.AppendLine("		INSERT INTO CoDiMa2_Dati (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		SELECT Id_Sensore, DataOra, Valore, 0 ")
                sql.AppendLine("		FROM @TMPTABLE tt ")
                sql.AppendLine("		WHERE tt.IsUpdate = 0 ")
                sql.AppendLine()
                sql.AppendLine("		SET @RESULT_INSERT = @@ROWCOUNT ")
                sql.AppendLine()
                sql.AppendLine("	END; ")
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
                sql.AppendLine("	THROW;  ")
                sql.AppendLine("END CATCH; ")
                sql.AppendLine()
                sql.AppendLine("SELECT [Update] = @RESULT_UPDATE, [Insert] = @RESULT_INSERT ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    nInsert += CInt(r("Insert"))
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoCoDiMa(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoCoDiMa"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Stazione = TRIM(Codice_Stazione), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Codice_Stazione, UltimaDataOra FROM ( ")
            sql.AppendLine("		SELECT sens.Codice_Stazione, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("		FROM CoDiMa2_Sensori sens ")
            sql.AppendLine("		INNER JOIN CoDiMa2_Stazioni staz ON staz.Codice = sens.Codice_Stazione AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("		LEFT JOIN ( ")
            sql.AppendLine("			SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM CoDiMa2_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("		) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	) T ")
            sql.AppendLine(") T GROUP BY Codice_Stazione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Agrismart"

    Public Function Upsert_Agrismart_Stations(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Agrismart_Stations"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Device] VARCHAR(20), [Label] VARCHAR(MAX), [Lat] REAL, [Lng] REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Device], [Label], [Lat], [Lng]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Agrismart_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Device] = s.[Device] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Device], [Label], [Lat], [Lng], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[Device], s.[Label], s.[Lat], s.[Lng], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Agrismart_Sensors(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Agrismart_Sensors"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Device] VARCHAR(20), [Sensor] VARCHAR(40), [Label] VARCHAR(MAX), [UM] varchar(10)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Device], [Sensor], [Label], [UM]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Agrismart_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Device] = s.[Device] AND t.[Sensor] = s.[Sensor] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Device], [Sensor], [Label], [UM]) ")
                sql.AppendLine("		VALUES (s.[Device], s.[Sensor], s.[Label], s.[UM]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Agrismart_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Agrismart_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Device VARCHAR(20), Sensor VARCHAR(40), DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOra = DATEADD(second, DATEDIFF(second, '20000101', DataOra), '20000101'), Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , Device ")
                sql.AppendLine("        , Sensor ")
                sql.AppendLine("    FROM Agrismart_Sensori ")
                sql.AppendLine(") s ON s.Device = tt.Device AND s.Sensor = tt.Sensor ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Agrismart_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore = AVG(Valore) FROM @SRCTABLE GROUP BY Id_Sensore, DataOra) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOra, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoAgrismart(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoAgrismart"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Device = TRIM(Device), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Device, UltimaDataOra FROM ( ")
            sql.AppendLine("    	SELECT sens.Device, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	    FROM Agrismart_Sensori sens ")
            sql.AppendLine("	    INNER JOIN Agrismart_Stazioni staz ON staz.Device = sens.Device AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM Agrismart_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	) T ")
            sql.AppendLine(") T GROUP BY Device ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Horta"

    Public Function Upsert_Horta_Stations(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Horta_Stations"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([QuadranteID] VARCHAR(20), [Nome] VARCHAR(MAX), [Lat] REAL, [Lng] REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([QuadranteID], [Nome], [Lat], [Lng]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Horta_Stazioni AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[QuadranteID] = s.[QuadranteID] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([QuadranteID], [Nome], [Lat], [Lng], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[QuadranteID], s.[Nome], s.[Lat], s.[Lng], 1) ")
                sql.AppendLine("        --Per le stazioni appena inserite attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Horta_Sensors(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Horta_Sensors"
        Dim nInsert As Integer = 0

        Try
            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([QuadranteID] VARCHAR(20), [Sensor] VARCHAR(40), [Label] VARCHAR(MAX), [UM] varchar(10)) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([QuadranteID], [Sensor], [Label], [UM]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Horta_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[QuadranteID] = s.[QuadranteID] AND t.[Sensor] = s.[Sensor] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([QuadranteID], [Sensor], [Label], [UM]) ")
                sql.AppendLine("		VALUES (s.[QuadranteID], s.[Sensor], s.[Label], s.[UM]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Horta_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Horta_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(QuadranteID VARCHAR(20), Sensor VARCHAR(40), DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOra = DATEADD(second, DATEDIFF(second, '20000101', DataOra), '20000101'), Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , QuadranteID ")
                sql.AppendLine("        , Sensor ")
                sql.AppendLine("    FROM Horta_Sensori ")
                sql.AppendLine(") s ON s.QuadranteID = tt.QuadranteID AND s.Sensor = tt.Sensor ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Horta_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore = AVG(Valore) FROM @SRCTABLE GROUP BY Id_Sensore, DataOra) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOra, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOra, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoHorta(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoHorta"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT QuadranteID = TRIM(QuadranteID), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT QuadranteID, UltimaDataOra FROM ( ")
            sql.AppendLine("    	SELECT sens.QuadranteID, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	    FROM Horta_Sensori sens ")
            sql.AppendLine("	    INNER JOIN Horta_Stazioni staz ON staz.QuadranteID = sens.QuadranteID AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOra) FROM Horta_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	) T ")
            sql.AppendLine(") T GROUP BY QuadranteID ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Hypermeteo condiviso"

    Public Function Leggi_Hypermeteo_Layers(ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.Leggi_Hypermeteo_Layers"

        Dim tbl As DataTable

        Try

            tbl = EseguiQuery_Lettura(ObjParametri_Server, "SELECT Id, NomeLayer FROM Hypermeteo_Tipi_Sensore", NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]: " & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return tbl
    End Function

    Public Function Upsert_Hypermeteo_Sensors(tbl_stazioni As String, tbl_sensori As String, ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Hypermeteo_Sensors"

        Dim nInsert As Integer = 0

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("INSERT INTO " & tbl_sensori)
            sql.AppendLine("SELECT")
            sql.AppendLine("    Id_Stazione = z.id, Id_Tipo_Sensore = ts.Id, Trasferibile = 1")
            sql.AppendLine("FROM " & tbl_stazioni & " z")
            sql.AppendLine("INNER JOIN Hypermeteo_Tipi_Sensore ts ON 1 = 1")
            sql.AppendLine("LEFT JOIN " & tbl_sensori & " s ON s.Id_Stazione = z.Id AND s.Id_Tipo_Sensore = ts.Id")
            sql.AppendLine("WHERE s.Id IS NULL")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione = 'INSERT', ContaOperazioni = @@ROWCOUNT")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function LeggiAreeControlloHypermeteo(tblAreeControllo As String, idGroup As Integer, DataOra As Date?, addHH As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.LeggiAreeControlloHypermeteo"

        Dim DT As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("; WITH AREA_CTRL_CTE AS (")
            sql.AppendLine("	SELECT")
            sql.AppendLine("		Id")
            sql.AppendLine(" 		, pt1 = Area.STPointN(1)")
            sql.AppendLine(" 		, pt2 = Area.STPointN(2)")
            sql.AppendLine(" 		, pt3 = Area.STPointN(3)")
            sql.AppendLine(" 		, pt4 = Area.STPointN(4)")

            If Not DataOra.HasValue Then
                sql.AppendLine("		, DataOraUTC = DATEADD(hh, " & addHH.ToString() & ", LastDateUTC)")
            Else
                sql.AppendLine("		, DataOraUTC = " & Agro_SQL_SaveDateTime(DataOra.Value, False))
            End If

            sql.AppendLine("	FROM " & tblAreeControllo)
            sql.AppendLine("	WHERE Active = 1")

            If idGroup > 0 Then
                sql.AppendLine("	AND IdGroup = " & idGroup.ToString)
            End If

            sql.AppendLine(")")
            sql.AppendLine()
            sql.AppendLine("SELECT T1.Id, ll_lat, ll_lng, ur_lat, ur_lng, T2.DataOraUTC")
            sql.AppendLine("FROM (")
            sql.AppendLine("	SELECT Id, ll_lat = MIN(lat), ll_lng = MIN(lng), ur_lat = MAX(lat), ur_lng = MAX(lng)")
            sql.AppendLine("	FROM (")
            sql.AppendLine("		SELECT Id, lat = pt1.Lat, lng = pt1.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("		UNION ALL")
            sql.AppendLine("		SELECT Id, lat = pt2.Lat, lng = pt2.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("		UNION ALL")
            sql.AppendLine("		SELECT Id, lat = pt3.Lat, lng = pt3.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("		UNION ALL")
            sql.AppendLine("		SELECT Id, lat = pt4.Lat, lng = pt4.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("	) T")
            sql.AppendLine("	GROUP BY Id")
            sql.AppendLine(") T1")
            sql.AppendLine("INNER JOIN AREA_CTRL_CTE T2 ON T2.Id = T1.Id")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)

        End Try

        Return DT
    End Function

#End Region


#Region "Gestione Hypermeteo"

    Public Sub Crea_Hypermeteo_DatiXTask(idTask As Integer, ObjParametri_Server As AgronicaCoreParametri)

        Dim NomeRoutine As String = "MeteoNT.Crea_Hypermeteo_DatiXTask"

        Try

            Dim tblName As String = "Hypermeteo_Dati_Task_" & idTask.ToString("00")

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("BEGIN TRY")
            sql.AppendLine()
            sql.AppendLine("	CREATE TABLE " & tblName & " (")
            sql.AppendLine("		Id_Sensore INTEGER NOT NULL REFERENCES Hypermeteo_Sensori (Id),")
            sql.AppendLine("		DataOraUTC DATETIME NOT NULL,")
            sql.AppendLine("		Valore REAL NULL,")
            sql.AppendLine("		Elaborato BIT NULL")
            sql.AppendLine("		PRIMARY KEY CLUSTERED (Id_Sensore ASC, DataOraUTC ASC)")
            sql.AppendLine("		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]")
            sql.AppendLine("	) ON [PRIMARY]")
            sql.AppendLine()
            sql.AppendLine("END TRY")
            sql.AppendLine("BEGIN CATCH")
            sql.AppendLine("END CATCH")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try
    End Sub

    Public Function Update_AreaControllo_LastDate(Id_Area As String, DataFine As DateTime, objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "MeteoNT.Update_AreaControllo_LastDate"

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("UPDATE Hypermeteo_Area_Control")
            sql.AppendLine("SET LastDateUTC = " & Agro_SQL_SaveDateTime(DataFine, False))
            sql.AppendLine("WHERE Id = " & Id_Area)

            Dim tbl = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]: " & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return True
    End Function

    Public Function Upsert_Hypermeteo_Dati(hlp As Upsert_Helper, sensDict As Dictionary(Of Integer, String), idTask As Integer, ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Hypermeteo_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim tblName As String = "Hypermeteo_Dati_Task_" & idTask.ToString("00")

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ImportRows INT;")
                sql.AppendLine("DECLARE @Updated INT = 0;")
                sql.AppendLine("DECLARE @Inserted INT = 0;")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ImportTable TABLE(Id_Sensore INT, DataOraUTC DateTime, Valore REAL)")
                sql.AppendLine("DECLARE @UpdatedTbl TABLE(Id_Sensore INT NOT NULL, DataOra DateTime NOT NULL);")
                sql.AppendLine()

                Dim declareTbl As String = "DECLARE @TMPTABLE TABLE(DataOraUTC Datetime, Lat REAL, Lng REAL"
                Dim insertTbl As String = "INSERT INTO @TMPTABLE (DataOraUTC, Lat, Lng"
                Dim caseStmt As String = "CASE s.Id_Tipo_Sensore"
                For Each kvp In sensDict
                    declareTbl &= ", " & kvp.Value & " REAL"
                    insertTbl &= ", " & kvp.Value
                    caseStmt &= " WHEN " & kvp.Key.ToString & " THEN " & kvp.Value
                Next
                declareTbl &= ")"
                insertTbl &= ")"
                caseStmt &= " END"

                sql.AppendLine(declareTbl)
                sql.AppendLine()
                sql.AppendLine(insertTbl)
                sql.AppendLine("VALUES")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("INSERT INTO @ImportTable (DataOraUTC, Id_Sensore, Valore)")
                sql.AppendLine("SELECT")
                sql.AppendLine("	t.DataOraUTC")
                sql.AppendLine("	, Id_Sensore = s.Id")
                sql.AppendLine("	, Valore = " & caseStmt)
                sql.AppendLine("FROM @TmpTable t")
                sql.AppendLine("INNER JOIN Hypermeteo_Stazioni z ON z.Quadrante.STIntersects(geography::Point(t.Lat, t.Lng, 4326)) = 1 ")
                sql.AppendLine("INNER JOIN Hypermeteo_Sensori s on s.Id_Stazione = z.Id AND s.Trasferibile = 1")
                sql.AppendLine()
                sql.AppendLine("SET @ImportRows = @@ROWCOUNT;")
                sql.AppendLine()
                sql.AppendLine("BEGIN TRY")
                sql.AppendLine()
                sql.AppendLine("	BEGIN TRAN;")
                sql.AppendLine()
                sql.AppendLine("	UPDATE hd SET hd.Valore = imp.Valore, Elaborato = 0")
                sql.AppendLine("	OUTPUT INSERTED.Id_Sensore, INSERTED.DataOraUTC INTO @UpdatedTbl (Id_Sensore, DataOra) ")
                sql.AppendLine("	FROM " & tblName & " hd")
                sql.AppendLine("	INNER JOIN @ImportTable imp ON imp.Id_Sensore = hd.Id_Sensore AND imp.DataOraUTC = hd.DataOraUTC")
                sql.AppendLine()
                sql.AppendLine("	SET @Updated = @@ROWCOUNT")
                sql.AppendLine()
                sql.AppendLine("	IF (@Updated < @ImportRows) -- if all rows were updates then skip, else insert remaining")
                sql.AppendLine("	BEGIN")
                sql.AppendLine("		-- get rid of rows that were updates, leaving only the ones to insert")
                sql.AppendLine("		DELETE src")
                sql.AppendLine("		FROM @ImportTable src")
                sql.AppendLine("		INNER JOIN @UpdatedTbl del ON del.Id_Sensore = src.Id_Sensore AND del.DataOra = src.DataOraUTC;")
                sql.AppendLine()
                sql.AppendLine("		INSERT INTO " & tblName & " (Id_Sensore, DataOraUTC, Valore, Elaborato)")
                sql.AppendLine("		SELECT src.Id_Sensore, src.DataOraUTC, src.Valore, 0")
                sql.AppendLine("		FROM @ImportTable src")
                sql.AppendLine()
                sql.AppendLine("		SET @Inserted = @@ROWCOUNT")
                sql.AppendLine("	END;")
                sql.AppendLine()
                sql.AppendLine("	COMMIT TRAN;")
                sql.AppendLine()
                sql.AppendLine("END TRY")
                sql.AppendLine("BEGIN CATCH")
                sql.AppendLine()
                sql.AppendLine("	IF (@@TRANCOUNT > 0)")
                sql.AppendLine("	BEGIN")
                sql.AppendLine("		ROLLBACK;")
                sql.AppendLine("	END;")
                sql.AppendLine()
                sql.AppendLine("	---- THROW; -- if using SQL 2012 or newer, use this and remove the following 3 lines")
                sql.AppendLine("	DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();")
                sql.AppendLine("	RAISERROR(@ErrorMessage, 16, 1);")
                sql.AppendLine("	RETURN;")
                sql.AppendLine("END CATCH;")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione = 'UPDATE', ContaOperazioni = @Updated")
                sql.AppendLine("UNION")
                sql.AppendLine("SELECT 'INSERT', @Inserted")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Delete_Hypermeteo_Dati_Elaborati(idTask As Integer, ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Try
            Dim tblName As String = "Hypermeteo_Dati_Task_" & idTask.ToString("00")

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @Counter INT = 1;")
            sql.AppendLine()
            sql.AppendLine("WHILE @Counter > 0")
            sql.AppendLine("BEGIN")
            sql.AppendLine()
            sql.AppendLine("    DELETE TOP(1000) D")
            sql.AppendLine("    FROM " & tblName & " D")
            sql.AppendLine("    WHERE Elaborato = 1 ")
            sql.AppendLine()
            sql.AppendLine("    SET @Counter = @@ROWCOUNT;")
            sql.AppendLine("END")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, "MeteoNT.Delete_Hypermeteo_Dati_Elaborati")

        Catch ex As Exception

        End Try

        Return True
    End Function

#End Region


#Region "Gestione Hypermeteo Previsionale"

    Public Sub Crea_HypermeteoPrevisionali_DatiXTask(idTask As Integer, ObjParametri_Server As AgronicaCoreParametri)

        Dim NomeRoutine As String = "MeteoNT.Crea_HypermeteoPrevisionali_DatiXTask"

        Try

            Dim tblName As String = "HypermeteoPrevisionali_Dati_Task_" & idTask.ToString("00")

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("BEGIN TRY")
            sql.AppendLine("	CREATE TABLE " & tblName & " (")
            sql.AppendLine("		Id_Sensore INTEGER NOT NULL REFERENCES HypermeteoPrevisionali_Sensori (Id),")
            sql.AppendLine("		DataOraUTC DATETIME NOT NULL,")
            sql.AppendLine("		Valore REAL NULL,")
            sql.AppendLine("		Elaborato BIT NULL")
            sql.AppendLine("		PRIMARY KEY CLUSTERED (Id_Sensore ASC, DataOraUTC ASC)")
            sql.AppendLine("		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]")
            sql.AppendLine("	) ON [PRIMARY]")
            sql.AppendLine("END TRY")
            sql.AppendLine("BEGIN CATCH")
            sql.AppendLine("	TRUNCATE TABLE " & tblName)
            sql.AppendLine("END CATCH")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try
    End Sub

    Public Function Upsert_Hypermeteo_Dati_Previsionali(hlp As Upsert_Helper, sensDict As Dictionary(Of Integer, String), idTask As Integer, ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_Hypermeteo_Dati_Previsionali"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim tblName As String = "HypermeteoPrevisionali_Dati_Task_" & idTask.ToString("00")

                Dim sql As New StringBuilder

                sql.Clear()

                If True Then

                    sql.AppendLine("SET NOCOUNT ON")
                    sql.AppendLine()

                    Dim declareTbl As String = "DECLARE @TMPTABLE TABLE(DataOraUTC Datetime, Lat REAL, Lng REAL"
                    Dim insertTbl As String = "INSERT INTO @TMPTABLE (DataOraUTC, Lat, Lng"
                    Dim caseStmt As String = "CASE s.Id_Tipo_Sensore"
                    For Each kvp In sensDict
                        declareTbl &= ", " & kvp.Value & " REAL"
                        insertTbl &= ", " & kvp.Value
                        caseStmt &= " WHEN " & kvp.Key.ToString & " THEN " & kvp.Value
                    Next
                    declareTbl &= ")"
                    insertTbl &= ")"
                    caseStmt &= " END"

                    sql.AppendLine(declareTbl)
                    sql.AppendLine()
                    sql.AppendLine(insertTbl)
                    sql.AppendLine("VALUES")
                    sql.Append(hlp.GetAndReset())
                    sql.AppendLine()
                    'sql.AppendLine("INSERT INTO HypermeteoPrevisionali_Dati (Id_Sensore, DataOraUTC, Valore, Elaborato, IdTask)")
                    sql.AppendLine("INSERT INTO " & tblName & " (Id_Sensore, DataOraUTC, Valore, Elaborato)")
                    'sql.AppendLine("SELECT Id_Sensore, DataOraUTC, Valore, 0, " & idGroup.ToString & " FROM (")
                    sql.AppendLine("SELECT Id_Sensore, DataOraUTC, Valore, 0 FROM (")
                    sql.AppendLine("	SELECT")
                    sql.AppendLine("		Id_Sensore = s.Id")
                    sql.AppendLine("		, t.DataOraUTC")
                    sql.AppendLine("		, Valore = " & caseStmt)
                    sql.AppendLine("	FROM @TMPTABLE t")
                    sql.AppendLine("	INNER JOIN HypermeteoPrevisionali_Stazioni z ON z.Quadrante.STIntersects(geography::Point(t.Lat, t.Lng, 4326)) = 1 ")
                    sql.AppendLine("	INNER JOIN HypermeteoPrevisionali_Sensori s on s.Id_Stazione = z.Id AND s.Trasferibile = 1")
                    sql.AppendLine(") T WHERE Valore IS NOT NULL")
                    sql.AppendLine()
                    sql.AppendLine("SELECT Operazione = 'INSERT', ContaOperazioni = @@ROWCOUNT")

                Else

                    sql.AppendLine("SET NOCOUNT ON")
                    sql.AppendLine()
                    sql.AppendLine(DropTmpTable("#TmpDati"))

                    Dim declareTbl As String = "CREATE TABLE #TmpDati (DataOraUTC Datetime NOT NULL, Lat REAL NOT NULL, Lng REAL NOT NULL"
                    Dim insertTbl As String = "INSERT INTO #TmpDati (DataOraUTC, Lat, Lng"
                    Dim caseStmt As String = "CASE s.Id_Tipo_Sensore"
                    For Each kvp In sensDict
                        declareTbl &= ", " & kvp.Value & " REAL NOT NULL"
                        insertTbl &= ", " & kvp.Value
                        caseStmt &= " WHEN " & kvp.Key.ToString & " THEN " & kvp.Value
                    Next
                    declareTbl &= ")"
                    insertTbl &= ")"
                    caseStmt &= " END"

                    sql.AppendLine(declareTbl)
                    sql.AppendLine()
                    sql.AppendLine(insertTbl)
                    sql.AppendLine("VALUES")
                    sql.Append(hlp.GetAndReset())
                    sql.AppendLine()
                    sql.AppendLine("INSERT INTO " & tblName & " (Id_Sensore, DataOraUTC, Valore, Elaborato)")
                    sql.AppendLine("SELECT Id_Sensore, DataOraUTC, Valore, 0 FROM (")
                    sql.AppendLine("	SELECT")
                    sql.AppendLine("		Id_Sensore = s.Id")
                    sql.AppendLine("		, d.DataOraUTC")
                    sql.AppendLine("		, Valore = " & caseStmt)
                    sql.AppendLine("	FROM #TmpDati d")
                    sql.AppendLine("	INNER JOIN HypermeteoPrevisionali_Stazioni z ON z.Quadrante.STIntersects(geography::Point(d.Lat, d.Lng, 4326)) = 1 ")
                    sql.AppendLine("	INNER JOIN HypermeteoPrevisionali_Sensori s on s.Id_Stazione = z.Id AND s.Trasferibile = 1")
                    sql.AppendLine(") T WHERE Valore IS NOT NULL")
                    sql.AppendLine()
                    sql.AppendLine("SELECT Operazione = 'INSERT', ContaOperazioni = @@ROWCOUNT")
                    sql.AppendLine()
                    sql.AppendLine(DropTmpTable("#TmpDati"))

                End If


                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next
            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function CancellaDatiPrevisionali(idGroup As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.CancellaDatiPrevisionali"
        Dim DT As DataTable = Nothing

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("; WITH AREA_CTRL_CTE AS (")
            sql.AppendLine("	SELECT")
            sql.AppendLine("		Id")
            sql.AppendLine(" 		, pt1 = Area.STPointN(1)")
            sql.AppendLine(" 		, pt2 = Area.STPointN(2)")
            sql.AppendLine(" 		, pt3 = Area.STPointN(3)")
            sql.AppendLine(" 		, pt4 = Area.STPointN(4)")
            sql.AppendLine("	FROM HypermeteoPrevisionali_Area_Control")
            If idGroup > 0 Then
                sql.AppendLine("	WHERE IdGroup = " & idGroup)
            End If
            sql.AppendLine("),")
            sql.AppendLine()
            sql.AppendLine("CORNER_CTRL_CTE AS (")
            sql.AppendLine("	SELECT T1.Id, ll_lat, ll_lng, ur_lat, ur_lng")
            sql.AppendLine("	FROM (")
            sql.AppendLine("		SELECT Id, ll_lat = MIN(lat), ll_lng = MIN(lng), ur_lat = MAX(lat), ur_lng = MAX(lng)")
            sql.AppendLine("		FROM (")
            sql.AppendLine("			SELECT Id, lat = pt1.Lat, lng = pt1.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("			UNION ALL")
            sql.AppendLine("			SELECT Id, lat = pt2.Lat, lng = pt2.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("			UNION ALL")
            sql.AppendLine("			SELECT Id, lat = pt3.Lat, lng = pt3.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("			UNION ALL")
            sql.AppendLine("			SELECT Id, lat = pt4.Lat, lng = pt4.Long FROM AREA_CTRL_CTE")
            sql.AppendLine("		) T")
            sql.AppendLine("		GROUP BY Id")
            sql.AppendLine("	) T1")
            sql.AppendLine("),")
            sql.AppendLine()
            sql.AppendLine("STAZIONI_CTE AS (")
            sql.AppendLine("	SELECT Id, Center = Quadrante.EnvelopeCenter() FROM HypermeteoPrevisionali_Stazioni")
            sql.AppendLine(")")
            sql.AppendLine()
            sql.AppendLine("DELETE D")
            sql.AppendLine("FROM MeteoNT_DatiOrari D")
            sql.AppendLine("INNER JOIN MeteoNT_Sensori S1 ON S1.Id = D.Id_Sensore")
            sql.AppendLine("INNER JOIN")
            sql.AppendLine("(")
            sql.AppendLine("	SELECT ChiaveImport = 'HYPERMETEO_PREV-' + CAST(S.Id_Stazione AS VARCHAR(10)) + '-' + CAST(S.Id_Tipo_Sensore AS VARCHAR(10))")
            sql.AppendLine("	FROM (")
            sql.AppendLine("		SELECT Id_Stazione = z.Id")
            sql.AppendLine("		FROM CORNER_CTRL_CTE cc")
            sql.AppendLine("		INNER JOIN AREA_CTRL_CTE a on a.Id = cc.Id")
            sql.AppendLine("		INNER JOIN STAZIONI_CTE z ON cc.ll_lat < z.Center.Lat AND z.Center.Lat < cc.ur_lat AND cc.ll_lng < z.Center.Long AND z.Center.Long < cc.ur_lng")
            sql.AppendLine("	) Z")
            sql.AppendLine("	INNER JOIN HypermeteoPrevisionali_Sensori S ON S.Id_Stazione = Z.Id_Stazione")
            sql.AppendLine(") S2 ON S2.ChiaveImport = S1.ChiaveImport")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


#Region "Gestione RadarMeteo_FTP"

    Public Function Upsert_RadarMeteoFTP_Stazioni(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer
        Return 0
    End Function

    Public Function Upsert_RadarMeteoFTP_Sensori(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer
        Return 0
    End Function

    Public Function Upsert_RadarMeteoFTP_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "MeteoNT.Upsert_RadarMeteoFTP_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(QuadranteID VARCHAR(25), Sensor VARCHAR(40), DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC = DATEADD(second, DATEDIFF(second, '20000101', DataOraUTC), '20000101'), Valore ")
                sql.AppendLine("FROM @TMPTABLE tt ")
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT ")
                sql.AppendLine("        Id ")
                sql.AppendLine("        , QuadranteID ")
                sql.AppendLine("        , Sensor ")
                sql.AppendLine("    FROM RadarMeteoFTP_Sensori ")
                sql.AppendLine(") s ON s.QuadranteID = tt.QuadranteID AND s.Sensor = tt.Sensor ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO RadarMeteoFTP_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT Id_Sensore, DataOraUTC, Valore = AVG(Valore) FROM @SRCTABLE GROUP BY Id_Sensore, DataOraUTC) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function StazioniXScaricoRadarMeteoFTP(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MeteoNT.StazioniXScaricoRadarMeteoFTP"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()
            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT QuadranteID = TRIM(QuadranteID), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT QuadranteID, UltimaDataOra FROM ( ")
            sql.AppendLine("    	SELECT sens.QuadranteID, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA) ")
            sql.AppendLine("	    FROM RadarMeteoFTP_Sensori sens ")
            sql.AppendLine("	    INNER JOIN RadarMeteoFTP_Stazioni staz ON staz.QuadranteID = sens.QuadranteID AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM RadarMeteoFTP_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("	) T ")
            sql.AppendLine(") T GROUP BY QuadranteID ")
            sql.AppendLine("ORDER BY UltimaDataOra ASC ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

#End Region


End Class
