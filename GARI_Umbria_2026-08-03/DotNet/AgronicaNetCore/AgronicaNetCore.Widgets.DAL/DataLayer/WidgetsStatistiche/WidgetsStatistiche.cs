using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Widgets.DAL.Resources;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsStatistiche.WidgetsStatistiche
{
    public class WidgetsStatistiche : BaseDALWidgets, IWidgetsStatistiche
    {
        /// <summary>
        /// Creates a temporaty table with just the visible companies' vat.
        /// Needs to be dropped with <see cref="SQL_DROP_VISIBILITY_FILTER"/> at the end of the query.
        /// </summary>
        private const string SQL_VISIBILITY_FILTER = @"
    CREATE TABLE #pive_visibili (
        PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
        Validita_Inizio datetime NOT NULL,
        Validita_Fine datetime NOT NULL
    )
    IF EXISTS (SELECT 1 FROM Utenti_Visibilita_Appoggio WHERE Username = @user)
        INSERT INTO #pive_visibili
        SELECT DISTINCT i.PIVA, i.Validita_Inizio, i.Validita_Fine 
        FROM Utenti_Visibilita_Appoggio uva
        INNER JOIN Imprese i ON uva.PIVA = i.PIVA
        WHERE uva.Username = @user
    ELSE
        INSERT INTO #pive_visibili SELECT DISTINCT PIVA, Validita_Inizio, Validita_Fine FROM Imprese
";

        private const string SQL_DROP_VISIBILITY_FILTER = "DROP TABLE #pive_visibili";


        public WidgetsStatistiche(IServiceProvider serviceProvider, IStringLocalizer<Messages> localizer) : base(serviceProvider, localizer) { }

        public async Task<DataTable?> GetCountriesAsync(int year, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (year == 0) throw new Exception("Selezionare un anno per cui estrarre i paesi");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            SELECT DISTINCT 
                                    stato AS code, country.descrizione AS descr
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.PIVA = ei.PIVA
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            INNER JOIN [ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166] country ON country.Codice = ind.stato
                            WHERE YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@year", year);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }

        public async Task<DataTable?> GetGeneralStatisticsAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            // LEGENDA:
            // FARMERS = IMPRESE
            // CAMPI = IMPIANTI
            // MAPPATI = ESISTE UN PUNTO GIS(LAYER 19 IMPIANTO)

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_FARMERS INT = 0
                            DECLARE @MAPPED_FARMERS INT = 0
                            DECLARE @NOT_MAPPED_FARMERS INT = 0

                            DECLARE @TOTAL_PLOTS INT = 0
                            DECLARE @MAPPED_PLOTS INT = 0

                            DECLARE @AVG_MAPPED_PLOTS_X_FARMER DECIMAL(15,2) = 0
                            DECLARE @TOTAL_HA DECIMAL(15,3) = 0
                            DECLARE @TOTAL_HA_MAPPED DECIMAL(15,3) = 0
                            DECLARE @AVG_HA_MAPPED_PLOTS_X_FARMER DECIMAL(15,2) = 0
                            DECLARE @AVG_HA_MAPPED_PLOTS DECIMAL(15,2) = 0


                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_MappedFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpMappedFarmer ON #tmp_MappedFarmer(PIVA) 
                            INSERT INTO #tmp_MappedFarmer
                            SELECT PIVA AS Chiave
                            FROM GIS_Entita e
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS
                            GROUP BY Piva

                            CREATE TABLE #tmp_CountryxFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxFarmer ON #tmp_CountryxFarmer(PIVA) 
                            INSERT INTO #tmp_CountryxFarmer
                            SELECT i.Piva 
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            GROUP BY i.PIVA


                            -------------------------
                            --  TEMP TABLES PLOTS  --
                            -------------------------
                            CREATE TABLE #tmp_MappedPlots (
                                Chiave nvarchar(200) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpMappedPlots ON #tmp_MappedPlots(Chiave) 
                            INSERT INTO #tmp_MappedPlots
                            SELECT DISTINCT PIVA + '_' + CONVERT(varchar, Sa_Cod) + '_' + CONVERT(varchar, Appezza) + '_' + CONVERT(varchar, Id_Imp) AS Chiave
                            FROM GIS_Entita e
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS

                            CREATE TABLE #tmp_CountryxPlot (
                                Chiave nvarchar(200) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
                                Sup_Imp FLOAT NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxPlot ON #tmp_CountryxPlot(Chiave, Sup_Imp) 
                            INSERT INTO #tmp_CountryxPlot
                            SELECT r.PIVA + '_' + CONVERT(varchar, Sa_Cod) + '_' + CONVERT(varchar, Appezza) + '_' + CONVERT(varchar, ID_REG), Sup_Imp
                            FROM Reg_Impianti r
                            INNER JOIN #pive_visibili i ON i.PIVA = r.PIVA
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            AND YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_FARMERS
                            SELECT 
                                @TOTAL_FARMERS = COUNT(tmp_Farmer.Piva)  
                            FROM #tmp_CountryxFarmer tmp_Farmer

                            -- @MAPPED_FARMERS
                            SELECT  
                                @MAPPED_FARMERS = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE EXISTS (SELECT Piva FROM #tmp_MappedFarmer WHERE #tmp_MappedFarmer.Piva = tmp_Farmer.PIVA)

                            -- @NOT_MAPPED_FARMERS
                            SELECT  
                                @NOT_MAPPED_FARMERS = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE NOT EXISTS (SELECT Piva FROM #tmp_MappedFarmer WHERE #tmp_MappedFarmer.Piva = tmp_Farmer.PIVA)

                            -- @TOTAL_PLOTS
                            SELECT 
                                @TOTAL_PLOTS = COUNT(tmp_Plot.Chiave)  
                            FROM #tmp_CountryxPlot tmp_Plot

                            -- @MAPPED_PLOTS
                            SELECT 
                                @MAPPED_PLOTS = COUNT(tmp_Plot.Chiave) 
                            FROM #tmp_CountryxPlot tmp_Plot 
                            WHERE EXISTS (SELECT Chiave FROM #tmp_MappedPlots WHERE #tmp_MappedPlots.Chiave = tmp_Plot.Chiave)

                            -- @AVG_MAPPED_PLOTS_X_FARMER
                            SELECT 
                                @AVG_MAPPED_PLOTS_X_FARMER = CASE WHEN @MAPPED_FARMERS > 0 
                                                      THEN CAST(@MAPPED_PLOTS / CAST(@MAPPED_FARMERS AS decimal (15,2)) AS decimal (15,2)) 
                                                      ELSE 0 END 

                            -- @TOTAL_HA
                            SELECT 
                                @TOTAL_HA = SUM(tmp_Plot.Sup_Imp) 
                            FROM #tmp_CountryxPlot tmp_Plot 

                            -- @TOTAL_HA_MAPPED
                            SELECT 
                                @TOTAL_HA_MAPPED = SUM(tmp_Plot.Sup_Imp) 
                            FROM #tmp_CountryxPlot tmp_Plot 
                            WHERE EXISTS (SELECT Chiave FROM #tmp_MappedPlots WHERE #tmp_MappedPlots.Chiave = tmp_Plot.Chiave)

                            -- @AVG_HA_MAPPED_PLOTS_X_FARMER
                            SELECT 
                                @AVG_HA_MAPPED_PLOTS_X_FARMER = CASE WHEN @MAPPED_FARMERS > 0 
                                                         THEN CAST(@TOTAL_HA_MAPPED / @MAPPED_FARMERS AS decimal (15,2)) 
                                                         ELSE 0 END 

                            -- @AVG_HA_MAPPED_PLOTS
                            SELECT 
                                @AVG_HA_MAPPED_PLOTS = CASE WHEN @MAPPED_PLOTS > 0 
                                                THEN CAST(@TOTAL_HA_MAPPED / @MAPPED_PLOTS AS decimal (15,2)) 
                                                ELSE 0 END 


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT @TOTAL_FARMERS AS TOTAL_FARMERS, 
                                   @MAPPED_FARMERS AS MAPPED_FARMERS,
                                   @NOT_MAPPED_FARMERS AS NOT_MAPPED_FARMERS, 

                                   --@TOTAL_PLOTS AS TOTAL_PLOTS,  -- FOR DEBUG NO NEED TO SHOW
                                   @MAPPED_PLOTS AS MAPPED_PLOTS, 

                                   @AVG_MAPPED_PLOTS_X_FARMER AS AVG_MAPPED_PLOTS_X_FARMER,
                                   --@TOTAL_HA AS TOTAL_HA,    -- FOR DEBUG NO NEED TO SHOW
                                   @TOTAL_HA_MAPPED AS TOTAL_HA_MAPPED,
                                   @AVG_HA_MAPPED_PLOTS_X_FARMER AS AVG_HA_MAPPED_PLOTS_X_FARMER,
                                   @AVG_HA_MAPPED_PLOTS AS AVG_HA_MAPPED_PLOTS
      
      
                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_MappedFarmer
                            DROP TABLE #tmp_CountryxFarmer
                            DROP TABLE #tmp_MappedPlots
                            DROP TABLE #tmp_CountryxPlot
" + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetMappedFarmersAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_FARMERS AS INT 
                            DECLARE @MAPPED_FARMERS AS DECIMAL(15,2)
                            DECLARE @PERC_MAPPED AS DECIMAL(15,2)


                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_MappedFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpMappedFarmer ON #tmp_MappedFarmer(PIVA) 
                            INSERT INTO #tmp_MappedFarmer
                            SELECT PIVA AS Chiave
                            FROM GIS_Entita e
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS
                            GROUP BY Piva

                            CREATE TABLE #tmp_CountryxFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxFarmer ON #tmp_CountryxFarmer(PIVA) 
                            INSERT INTO #tmp_CountryxFarmer
                            SELECT i.Piva 
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            GROUP BY i.PIVA


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_FARMERS
                            SELECT 
                                @TOTAL_FARMERS = COUNT(tmp_Farmer.Piva)  
                            FROM #tmp_CountryxFarmer tmp_Farmer

                            -- @MAPPED_FARMERS
                            SELECT  
                                @MAPPED_FARMERS = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE EXISTS (SELECT Piva FROM #tmp_MappedFarmer WHERE #tmp_MappedFarmer.Piva = tmp_Farmer.PIVA)

                            -- @@PERC_MAPPED
                            SELECT @PERC_MAPPED = @MAPPED_FARMERS * 100 / @TOTAL_FARMERS


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT @PERC_MAPPED AS PERC_MAPPED, 100 - @PERC_MAPPED AS PERC_NOT_MAPPED               


                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_MappedFarmer
                            DROP TABLE #tmp_CountryxFarmer
" + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetMovedMappedFarmersPriorWeekAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_MAPPED_FARMERS AS INT 
                            DECLARE @MAPPED_FARMERS_MOVED AS DECIMAL(15,2) 
                            DECLARE @PERC_MOVED AS DECIMAL(15,2)


                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_CountryxMappedFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxFarmer ON #tmp_CountryxMappedFarmer(PIVA) 
                            INSERT INTO #tmp_CountryxMappedFarmer
                            SELECT i.Piva 
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            INNER JOIN GIS_Entita e ON e.Piva = i.PIVA
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            GROUP BY i.PIVA


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_MAPPED_FARMERS
                            SELECT 
                                @TOTAL_MAPPED_FARMERS = COUNT(tmp_MappedFarmer.Piva)  
                            FROM #tmp_CountryxMappedFarmer tmp_MappedFarmer

                            -- @MAPPED_FARMERS_MOVED
                            ;WITH cte_Movimenti AS (SELECT DISTINCT Piva 
                                                    FROM Movimenti
                                                    WHERE Data_Movimento >= DATEADD(DAY, -7, GETDATE()) AND Data_Movimento <= GETDATE())
                            SELECT 
                                @MAPPED_FARMERS_MOVED = COUNT(tmp_MappedFarmer.Piva) 
                            FROM #tmp_CountryxMappedFarmer tmp_MappedFarmer
                            WHERE EXISTS (SELECT Piva FROM cte_Movimenti WHERE cte_Movimenti.PIVA = tmp_MappedFarmer.PIVA)

                            -- @PERC_MOVED
                            SELECT @PERC_MOVED = @MAPPED_FARMERS_MOVED * 100 / @TOTAL_MAPPED_FARMERS


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT @PERC_MOVED AS PERC_MOVED, 100 - @PERC_MOVED AS PERC_NOT_MOVED


                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_CountryxMappedFarmer
                            " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetMovedMappedFarmersCampaignBeginAsync(int year, string country, DateTime campaignBegin, DateTime campaignEnd, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_MAPPED_FARMERS AS INT 
                            DECLARE @MAPPED_FARMERS_MOVED AS DECIMAL(15,2) 
                            DECLARE @PERC_MOVED AS DECIMAL(15,2)


                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_CountryxMappedFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxFarmer ON #tmp_CountryxMappedFarmer(PIVA) 
                            INSERT INTO #tmp_CountryxMappedFarmer
                            SELECT i.Piva 
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            INNER JOIN GIS_Entita e ON e.Piva = i.PIVA
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            GROUP BY i.PIVA


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_MAPPED_FARMERS
                            SELECT 
                                @TOTAL_MAPPED_FARMERS = COUNT(tmp_MappedFarmer.Piva)  
                            FROM #tmp_CountryxMappedFarmer tmp_MappedFarmer

                            -- @MAPPED_FARMERS_MOVED
                            ;WITH cte_Movimenti AS (SELECT DISTINCT Piva 
                                                    FROM Movimenti
                                                    WHERE Data_Movimento >= CONVERT(Datetime, @campaignBegin, 120) AND Data_Movimento <= CONVERT(Datetime, @campaignEnd, 120))
                            SELECT 
                                @MAPPED_FARMERS_MOVED = COUNT(tmp_MappedFarmer.Piva) 
                            FROM #tmp_CountryxMappedFarmer tmp_MappedFarmer
                            WHERE EXISTS (SELECT Piva FROM cte_Movimenti WHERE cte_Movimenti.PIVA = tmp_MappedFarmer.PIVA)

                            -- @PERC_MOVED
                            SELECT @PERC_MOVED = @MAPPED_FARMERS_MOVED * 100 / @TOTAL_MAPPED_FARMERS


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT @PERC_MOVED AS PERC_MOVED, 100 - @PERC_MOVED AS PERC_NOT_MOVED

                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_CountryxMappedFarmer
" + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@campaignBegin", campaignBegin);
                parSql.Add("@campaignEnd", campaignEnd);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetCropMapAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_HA DECIMAL(15,3) = 0
                            DECLARE @Top5Species TABLE (Veg_Cod INT NOT NULL);

                            CREATE TABLE #tmp_Plot (
                                Piva nvarchar(200) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
                                Sup_Imp FLOAT NOT NULL,
                                CUL_COD INT NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_Plot ON #tmp_Plot(Piva, Sup_Imp, CUL_COD) 
                            INSERT INTO #tmp_Plot
                            SELECT r.PIVA, r.Sup_Imp, r.CUL_COD 
                                FROM Reg_Impianti r
                                WHERE YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @Top5Species
                            ; WITH cte_Country AS (
                                SELECT DISTINCT i.PIVA FROM #pive_visibili i 
                                INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                                INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                                WHERE ind.stato = @country
                                AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            ) 
                            INSERT INTO @Top5Species(Veg_Cod)
                            SELECT TOP 5
                                ISNULL(s.Veg_Cod, 0)
                            FROM #tmp_Plot r
                            INNER JOIN cte_Country ii on ii.PIVA = r.PIVA
                            LEFT JOIN Cultivar c ON c.Cul_Cod = r.CUL_COD
                            LEFT JOIN SpecieVegetali s ON s.Veg_Cod = c.Veg_Cod
                            GROUP BY ISNULL(s.Veg_Cod, 0)
                            ORDER BY SUM(Sup_Imp) DESC

                             -- @TOTAL_HA
                            ; WITH cte_Country AS (
                                SELECT DISTINCT i.PIVA FROM #pive_visibili i 
                                INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                                INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                                WHERE ind.stato = @country
                                AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            ) 
                            SELECT 
                                 @TOTAL_HA = SUM(tmp_Plot.Sup_Imp) 
                             FROM #tmp_Plot tmp_Plot 
                             INNER JOIN cte_Country ii on ii.PIVA = tmp_Plot.PIVA


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            ;WITH cte_Country AS (
                                SELECT DISTINCT i.PIVA FROM #pive_visibili i 
                                INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                                INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                                WHERE ind.stato = @country
                                AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            )
                            SELECT * FROM (
                                SELECT
                                    CASE WHEN s.Veg_Cod IS NULL
                                        THEN 'Altri Utilizzi' 
                                        ELSE Veg_Des 
                                    END AS Usage_Des, 
                                    SUM(Sup_Imp) Tot_Ha_xUsage, 
                                    SUM(Sup_Imp) * 100 / @TOTAL_HA AS Perc
                                FROM #tmp_Plot r
                                INNER JOIN cte_Country ii on ii.PIVA = r.PIVA
                                LEFT JOIN Cultivar c ON c.Cul_Cod = r.CUL_COD
                                LEFT JOIN SpecieVegetali s ON s.Veg_Cod = c.Veg_Cod
                                WHERE ISNULL(s.Veg_Cod, 0) IN (SELECT Veg_Cod FROM @Top5Species)
                                GROUP BY s.Veg_Cod, s.Veg_Des
                            UNION
                                SELECT
                                    'Rimanenti Colture' AS Usage_Des, 
                                    ISNULL(SUM(Sup_Imp), -1) Tot_Ha_xUsage, 
                                    ISNULL(SUM(Sup_Imp) * 100 / @TOTAL_HA, -1) AS Perc
                                FROM #tmp_Plot r
                                INNER JOIN cte_Country ii on ii.PIVA = r.PIVA
                                LEFT JOIN Cultivar c ON c.Cul_Cod = r.CUL_COD
                                LEFT JOIN SpecieVegetali s ON s.Veg_Cod = c.Veg_Cod
                                WHERE  ISNULL(s.Veg_Cod, 0) NOT IN (SELECT Veg_Cod FROM @Top5Species)
                            ) X
                            ORDER BY x.Perc


                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_Plot
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetFarmersHarvestSowingDataAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_FARMERS AS INT 

                            DECLARE @FARMERS_SOWINGCOUNT AS DECIMAL(15,2)
                            DECLARE @PERC_SOWING AS DECIMAL(15,2)

                            DECLARE @FARMERS_HARVESTCOUNT AS DECIMAL(15,2)
                            DECLARE @PERC_HARVEST AS DECIMAL(15,2)



                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_MappedFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpMappedFarmer ON #tmp_MappedFarmer(PIVA) 
                            INSERT INTO #tmp_MappedFarmer
                            SELECT PIVA AS Chiave
                            FROM GIS_Entita e
                            INNER JOIN GIS_ElementiGrafici eg ON e.Entita_Cod = eg.Entita_Cod AND eg.PivaSuperUser = e.PivaSuperUser and eg.LayerElementiGrafici_Cod = @layerGIS
                            GROUP BY Piva

                            CREATE TABLE #tmp_CountryxFarmer (
                                PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxFarmer ON #tmp_CountryxFarmer(PIVA) 
                            INSERT INTO #tmp_CountryxFarmer
                            SELECT i.Piva 
                            FROM  #pive_visibili i
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            GROUP BY i.PIVA


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_FARMERS
                            SELECT  
                                @TOTAL_FARMERS = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE EXISTS (SELECT Piva FROM #tmp_MappedFarmer WHERE #tmp_MappedFarmer.Piva = tmp_Farmer.PIVA)


                            -- @FARMERS_SOWINGCOUNT
                            ;WITH cte_Sowing AS (SELECT DISTINCT a.Piva 
                                                 FROM Agenda a
                                                 INNER JOIN Movimenti m ON m.Id_Agenda = a.Id_Agenda
                                                 WHERE YEAR(Data_Movimento) = @year 
                                                 AND m.CAU_MOV = @CAU_LAVORAZIONE
                                                 AND a.Lav_Cod IN (@LAVCOD_SEMINA, @LAVCOD_TRAPIANTO, @LAVCOD_SOVESCIO, @LAVCOD_SOD_SEDDING))
                            SELECT 
                                @FARMERS_SOWINGCOUNT = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE EXISTS (SELECT Piva FROM cte_Sowing WHERE cte_Sowing.PIVA = tmp_Farmer.PIVA)

                            -- @PERC_SOWING
                            SELECT @PERC_SOWING = @FARMERS_SOWINGCOUNT * 100 / @TOTAL_FARMERS


                            -- @FARMERS_HARVESTCOUNT
                            ;WITH cte_Harvest AS (SELECT DISTINCT a.Piva 
                                                  FROM Agenda a
                                                  INNER JOIN Movimenti m ON m.Id_Agenda = a.Id_Agenda
                                                  WHERE YEAR(Data_Movimento) = @year 
                                                  AND m.CAU_MOV = @CAU_RILIEVO_RACCOLTA 
                                                  AND a.Lav_Cod IN (@LAVCOD_RACCOLTA)) 
                            SELECT 
                                @FARMERS_HARVESTCOUNT = COUNT(tmp_Farmer.Piva) 
                            FROM #tmp_CountryxFarmer tmp_Farmer
                            WHERE EXISTS (SELECT Piva FROM cte_Harvest WHERE cte_Harvest.PIVA = tmp_Farmer.PIVA)

                            -- @PERC_HARVEST
                            SELECT @PERC_HARVEST = @FARMERS_HARVESTCOUNT * 100 / @TOTAL_FARMERS


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT 
                                @TOTAL_FARMERS AS TOTAL_FARMERS,
                                @FARMERS_SOWINGCOUNT AS FARMERS_SOWINGCOUNT,
                                @PERC_SOWING AS PERC_SOWING,
                                @FARMERS_HARVESTCOUNT AS FARMERS_HARVESTCOUNT,
                                @PERC_HARVEST AS PERC_HARVEST

                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_MappedFarmer
                            DROP TABLE #tmp_CountryxFarmer
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                parSql.Add("@LAVCOD_SEMINA", LAV_COD.LAVCOD_SEMINA);
                parSql.Add("@LAVCOD_SOD_SEDDING", LAV_COD.LAVCOD_SOD_SEDDING);
                parSql.Add("@LAVCOD_TRAPIANTO", LAV_COD.LAVCOD_TRAPIANTO);
                parSql.Add("@LAVCOD_SOVESCIO", LAV_COD.LAVCOD_SOVESCIO);
                parSql.Add("@LAVCOD_RACCOLTA", LAV_COD.LAVCOD_RACCOLTA);

                parSql.Add("@CAU_RILIEVO_RACCOLTA", CAU_MOV.CAU_RILIEVO_RACCOLTA);
                parSql.Add("@CAU_LAVORAZIONE", CAU_MOV.CAU_LAVORAZIONE);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetPlotsHarvestSowingDataAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------
                            DECLARE @TOTAL_HA AS DECIMAL(15,2) 

                            DECLARE @HA_SOWING AS DECIMAL(15,2)
                            DECLARE @PERC_SOWING AS DECIMAL(15,2)

                            DECLARE @HA_HARVEST AS DECIMAL(15,2)
                            DECLARE @PERC_HARVEST AS DECIMAL(15,2)


                            --------------------------
                            --  TEMP TABLES FARMER  --
                            --------------------------
                            CREATE TABLE #tmp_CountryxPlot (
                                Chiave nvarchar(200) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
                                Sup_Imp FLOAT NOT NULL
                            )
                            CREATE CLUSTERED INDEX PK_tmpCountryxPlot ON #tmp_CountryxPlot(Chiave, Sup_Imp) 
                            INSERT INTO #tmp_CountryxPlot
                            SELECT r.PIVA + '_' + CONVERT(varchar, Sa_Cod) + '_' + CONVERT(varchar, Appezza) + '_' + CONVERT(varchar, ID_REG), Sup_Imp
                            FROM Reg_Impianti r
                            INNER JOIN #pive_visibili i ON i.PIVA = r.PIVA
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            AND YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year


                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TOTAL_HA
                            SELECT 
                                @TOTAL_HA = SUM(tmp_Plot.Sup_Imp) 
                            FROM #tmp_CountryxPlot tmp_Plot 

                            -- @HA_SOWING
                            ;WITH cte_Sowing AS (SELECT 
                                                    DISTINCT a.PIVA + '_' + CONVERT(varchar, a.Sa_Cod) + '_' + CONVERT(varchar, md.Appezza) + '_' + CONVERT(varchar, Id_Destinazione) AS Chiave
                                                 FROM Agenda a
                                                 INNER JOIN Movimenti m ON m.Id_Agenda = a.Id_Agenda
                                                 INNER JOIN Mov_Destinazioni md ON md.Id_Agenda = m.Id_Agenda AND  md.Id_Mov = m.Id_Mov 
                                                 WHERE YEAR(Data_Movimento) = @year 
                                                 AND m.CAU_MOV = @CAU_LAVORAZIONE
                                                 AND a.Lav_Cod IN (@LAVCOD_SEMINA, @LAVCOD_TRAPIANTO, @LAVCOD_SOVESCIO, @LAVCOD_SOD_SEDDING)
                                                 AND Id_Destinazione <> 0)
                            SELECT 
                                @HA_SOWING = SUM(tmp_Plot.Sup_Imp) 
                            FROM #tmp_CountryxPlot tmp_Plot
                            JOIN cte_Sowing ON cte_Sowing.Chiave = tmp_Plot.Chiave

                            -- @PERC_SOWING
                            SELECT @PERC_SOWING = @HA_SOWING * 100 / @TOTAL_HA


                            -- @HA_HARVEST
                            ;WITH cte_Harvest AS (SELECT DISTINCT a.PIVA + '_' + CONVERT(varchar, a.Sa_Cod) + '_' + CONVERT(varchar, md.Appezza) + '_' + CONVERT(varchar, Id_Destinazione) AS Chiave
                                                 FROM Agenda a
                                                 INNER JOIN Movimenti m ON m.Id_Agenda = a.Id_Agenda
                                                 INNER JOIN Mov_Destinazioni md ON md.Id_Agenda = m.Id_Agenda AND  md.Id_Mov = m.Id_Mov 
                                                 WHERE YEAR(Data_Movimento) = @year 
                                                 AND m.CAU_MOV = @CAU_RILIEVO_RACCOLTA
                                                 AND a.Lav_Cod IN (@LAVCOD_RACCOLTA)
                                                 AND Id_Destinazione <> 0)
                            SELECT 
                                @HA_HARVEST = SUM(tmp_Plot.Sup_Imp) 
                            FROM #tmp_CountryxPlot tmp_Plot
                            JOIN cte_Harvest ON cte_Harvest.Chiave = tmp_Plot.Chiave

                            -- @PERC_HARVEST
                            SELECT @PERC_HARVEST = @HA_HARVEST * 100 / @TOTAL_HA


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT 
                                @TOTAL_HA AS TOTAL_HA,
                                @HA_SOWING AS HA_SOWING,
                                @PERC_SOWING AS PERC_SOWING,
                                @HA_HARVEST AS HA_HARVES,
                                @PERC_HARVEST AS PERC_HARVEST


                            ------------------------
                            --  DROP TEMP TABLES  --
                            ------------------------
                            DROP TABLE #tmp_CountryxPlot
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@layerGIS", Enum_Gis_LayerElementiGrafici_std.IMPIANTI);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                parSql.Add("@LAVCOD_SEMINA", LAV_COD.LAVCOD_SEMINA);
                parSql.Add("@LAVCOD_SOD_SEDDING", LAV_COD.LAVCOD_SOD_SEDDING);
                parSql.Add("@LAVCOD_TRAPIANTO", LAV_COD.LAVCOD_TRAPIANTO);
                parSql.Add("@LAVCOD_SOVESCIO", LAV_COD.LAVCOD_SOVESCIO);
                parSql.Add("@LAVCOD_RACCOLTA", LAV_COD.LAVCOD_RACCOLTA);

                parSql.Add("@CAU_RILIEVO_RACCOLTA", CAU_MOV.CAU_RILIEVO_RACCOLTA);
                parSql.Add("@CAU_LAVORAZIONE", CAU_MOV.CAU_LAVORAZIONE);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetTargetHAAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;
            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                            -----------------
                            --  VARIABLES  --
                            -----------------

                            DECLARE @TARGET_HA AS DECIMAL(15,2) 
                            DECLARE @TOTAL_HA AS DECIMAL(15,2) 

                            -----------------------
                            --  DATA EXTRACTION  --
                            ----------------------- 
                            -- @TARGET_HA
                            SELECT 
                                @TARGET_HA = ISNULL(SUM(Sup_Imp), -1)
                            FROM Budget_Testata t 
                            INNER JOIN Budget_Reg_Impianti r ON r.Id_Budget = t.Id_Budget
                            INNER JOIN #pive_visibili i ON i.PIVA = r.PIVA
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE t.In_Uso = 1
                            AND Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            AND YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year

                            -- @TOTAL_HA
                            SELECT 
                                @TOTAL_HA = SUM(Sup_Imp)
                            FROM Reg_Impianti r
                            INNER JOIN #pive_visibili i ON i.PIVA = r.PIVA
                            INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                            INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                            WHERE Stato = @country
                            AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                            AND YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year


                            ---------------------------
                            --  DATATABLE TO RETURN  --
                            ---------------------------
                            SELECT 
                                @TARGET_HA AS TARGET_HA,
                                @TOTAL_HA AS TOTAL_HA
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
        public async Task<DataTable?> GetFarmerxRegionxRangeAsync(int year, string country, AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;
            try
            {
                if (string.IsNullOrEmpty(country))
                    throw new ArgumentException("Specificare un country");

                if (year == 0) throw new ArgumentException("Selezionare un anno per cui estrarre le statistiche");

                string? strSql = SQL_VISIBILITY_FILTER + @"
                                    -----------------
                                    --  VARIABLES  --
                                    -----------------
                                    DECLARE @Top3Region TABLE (Region nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL, PlotCount INT NOT NULL);
                                    DECLARE @RangeHA TABLE (code INT, descr nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL);

                                    --RANGE HA AREA
                                    --1 = 0.1ha to 0.5ha
                                    --2 = 0.51ha to 1ha
                                    --3 = 1ha to 2ha
                                    --4 = 2ha to 5ha
                                    --5 = 5ha to 10ha 
                                    --6 = 11ha to 20ha
                                    --7 = 21ha to 50ha
                                    --8 = > 50ha


                                    -------------------------
                                    --  TEMP TABLES PLOTS  --
                                    -------------------------
                                    CREATE TABLE #tmp_PlotxRegion (
                                        PIVA nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
                                        Region nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NOT NULL,
                                        HAGroup INT NOT NULL
                                    )
                                    CREATE CLUSTERED INDEX PK_tmpPlotxRegion ON #tmp_PlotxRegion(PIVA, Region, HAGroup) 
                                    INSERT INTO #tmp_PlotxRegion
                                    SELECT r.PIVA, lr.REG, 
                                    CASE WHEN Sup_Imp <= 0.5 THEN 1
                                         WHEN Sup_Imp BETWEEN 0.5000001 AND 1 THEN 2
                                         WHEN Sup_Imp BETWEEN 1.000001 AND 2 THEN 3

                                         WHEN Sup_Imp BETWEEN 2.000001 AND 5 THEN 4
                                         WHEN Sup_Imp BETWEEN 5.000001 AND 10 THEN 5
                                         WHEN Sup_Imp BETWEEN 10.000001 AND 20 THEN 6
                                         WHEN Sup_Imp BETWEEN 20.000001 AND 50 THEN 7
                                         WHEN Sup_Imp >= 50.000001 THEN 8 
                                    END AS HAGroup
                                    FROM Reg_Impianti r
                                    INNER JOIN #pive_visibili i ON i.PIVA = r.PIVA
                                    INNER JOIN ImpresexIndirizzi ei ON i.piva=ei.piva
                                    INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ei.cod_indirizzo
                                    LEFT OUTER JOIN ISTAT ON (ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM) 
                                    LEFT OUTER JOIN Lista_Province lp ON lp.Sigla = ISTAT.COMUNI_PROV 
                                    LEFT OUTER JOIN Lista_Regioni lr ON lr.REG = lp.REG 
                                    WHERE Stato = @country
                                    AND YEAR(i.Validita_Inizio) <= @year AND YEAR(i.Validita_Fine) >= @year
                                    AND YEAR(r.Validita_Inizio) <= @year AND YEAR(r.Validita_Fine) >= @year


                                    -----------------------
                                    --  DATA EXTRACTION  --
                                    ----------------------- 
                                    -- @Top3Region
                                    INSERT INTO @Top3Region(Region, PlotCount)
                                    SELECT TOP 3
                                         Region, COUNT(1) AS PlotCount
                                    FROM #tmp_PlotxRegion tmp_PlotxRegion
                                    GROUP BY Region
                                    ORDER BY COUNT(1) DESC

                                    INSERT INTO @RangeHA(code, descr)
                                    VALUES  (1, 'Da 0,1 a 0,5 ha'), (2, 'Da 0,51 a 1 ha'), (3, 'Da 1 a 2 ha'),
                                            (4, 'Da 2 a 5 ha'), (5, 'Da 5 a 10 ha'), (6, 'Da 11 a 20 ha'), (7, 'Da 21 a 50 ha'), (8, '> 50')


                                    ---------------------------
                                    --  DATATABLE TO RETURN  --
                                    ---------------------------
                                    SELECT 
                                        ISNULL(CountFarmers, 0) AS CountFarmersxRegionxRange
                                        , lr.Regione_Des AS AdminArea
                                        , RegionXRangeHA.RangeHACode
                                    FROM (
                                        SELECT 
                                            COUNT(PIVA) AS CountFarmers, tmp_PlotxRegion.Region, tmp_PlotxRegion.HAGroup AS RangeHACode
                                        FROM #tmp_PlotxRegion tmp_PlotxRegion
                                        INNER JOIN Lista_Regioni lr ON lr.REG = tmp_PlotxRegion.Region
                                        WHERE Region IN (SELECT Region FROM @Top3Region)
                                        GROUP BY tmp_PlotxRegion.Region, tmp_PlotxRegion.HAGroup
                                        ) AS FarmerxRangeHaxRegion
                                    RIGHT OUTER JOIN (SELECT DISTINCT R.Region, H.Code AS RangeHACode
                                                      FROM #tmp_PlotxRegion R 
                                                      CROSS JOIN @RangeHA H 
                                                      WHERE Region IN (SELECT Region FROM @Top3Region)) RegionXRangeHA ON RegionXRangeHA.Region = FarmerxRangeHaxRegion.Region AND FarmerxRangeHaxRegion.RangeHACode = RegionXRangeHA.RangeHACode
                                    LEFT OUTER JOIN Lista_Regioni lr ON lr.REG = RegionXRangeHA.Region
                                    ORDER BY RegionXRangeHA.Region, RegionXRangeHA.RangeHACode

  
                                    ------------------------
                                    --  DROP TEMP TABLES  --
                                    ------------------------
                                    DROP TABLE #tmp_PlotxRegion
                " + SQL_DROP_VISIBILITY_FILTER;

                parSql.Add("@country", country);
                parSql.Add("@year", year);
                parSql.Add("@user", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
    }
}
