using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    /// Classe DAL per le operazioni di calcolo dei centroidi.
    public class GisClustering : BaseDALGis, IGisClustering
    {
        public GisClustering(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        public async Task<(int, string)[]> GetElementoGraficoWktsAsync(string pivaSuperUser, int[] elementoGraficoCods, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parametriInSql = new Dictionary<string, Dictionary<Type, List<object>>>();
            DataTable result;

            // Query per recuperare il WKT dalla tabella principale degli elementi grafici.
            // Query modificata per gestire sia geometrie semplici con WKT pre-calcolato
            // sia geometrie complesse dove il WKT viene calcolato al momento.
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("   CASE WHEN Poligono_GeoEntity_WKT IS NULL THEN");
            stbQuery.AppendLine("       Poligono_GeoEntity.STAsText()");
            stbQuery.AppendLine("   ELSE");
            stbQuery.AppendLine("       Poligono_GeoEntity_WKT");
            stbQuery.AppendLine("   END AS Poligono_GeoEntity_WKT," +
                                "   ElementoGrafico_Cod ");
            stbQuery.AppendLine(" FROM dbo.GIS_ElementiGrafici");
            stbQuery.AppendLine(" WHERE PivaSuperUser = @PivaSuperUser");
            stbQuery.AppendLine("   AND ElementoGrafico_Cod IN (@elementoGraficoCods)");
            stbQuery.AppendLine("   AND Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("   AND Validita_Fine >= @dtInizio");
            stbQuery.AppendLine("   AND inviato >= 0");

            parametriSql.Add("@PivaSuperUser", pivaSuperUser);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriInSql.Add("@elementoGraficoCods", new Dictionary<Type, List<object>> { { typeof(int), elementoGraficoCods.Cast<object>().ToList() } });

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql, parametriInSql);
                var res = new List<(int, string)>();
                foreach (DataRow row in result.Rows)
                {
                    var wkt = row["Poligono_GeoEntity_WKT"].ToString();
                    if (!string.IsNullOrWhiteSpace(wkt))
                    {
                        res.Add(((int)row["ElementoGrafico_Cod"], wkt));
                    }
                }

                return res.ToArray();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return null;
        }

        public async Task<bool> UpdateClusteringRecordAsync(string pivaSuperUser, int layerCod, int elementoGraficoCod, string centroideWkt, string username, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();
            bool success;

            stbQuery.AppendLine(" UPDATE dbo.GIS_ElementiGrafici_Clustering");
            stbQuery.AppendLine(" SET Centroide_GeoEntity_WKT = @CentroideWKT,");
            stbQuery.AppendLine("     Centroide_GeoEntity = geography::STGeomFromText(@CentroideWKT, 4326),");
            stbQuery.AppendLine("     Stato = 1,"); // Stato 1 = Processato
            stbQuery.AppendLine("     Data_Modifica = GETDATE(),");
            stbQuery.AppendLine("     Username_Modifica = @Username");
            stbQuery.AppendLine(" WHERE PivaSuperUser = @PivaSuperUser");
            stbQuery.AppendLine("   AND LayerElementoGrafico_Cod = @LayerCod");
            stbQuery.AppendLine("   AND ElementoGrafico_Cod = @ElementoGraficoCod");

            parametriSql.CentroideWKT = centroideWkt;
            parametriSql.Username = username;
            parametriSql.PivaSuperUser = pivaSuperUser;
            parametriSql.LayerCod = layerCod;
            parametriSql.ElementoGraficoCod = elementoGraficoCod;

            try
            {
                success = await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return success;
        }
        

        public async Task<bool> UpdateClusteringRecordsAsync(string pivaSuperUser, int layerCod, (int, string)[] elements, string username, AgronicaCoreParametriServer objParametriServer)
        {
            bool success;

            var additionalFilter = $"t.PivaSuperUser = {pivaSuperUser} AND t.LayerElementoGrafico_Cod = {layerCod}";

            var dt = new DataTable();
            dt.Columns.Add("ElementoGrafico_Cod", typeof(int));
            dt.Columns.Add("Centroide_GeoEntity_WKT", typeof(string));
            dt.Columns.Add("Centroide_GeoEntity", typeof(string));
            dt.Columns.Add("Stato", typeof(int));
            dt.Columns.Add("Data_Modifica", typeof(DateTime));
            dt.Columns.Add("Username_Modifica", typeof(string));

            foreach (var element in elements)
            {
                dt.Rows.Add(element.Item1, element.Item2, element.Item2, 1, DateTime.Now, username);
            }
            
            try
            {
                success = await GetDataProvider(objParametriServer).ExecuteBulkUpdateAsync(dt, "GIS_ElementiGrafici_Clustering", "ElementoGrafico_Cod", additionalFilter: additionalFilter);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return success;
        }

        public async Task<DataTable> GetClusteringRecordsToProcessAsync(string pivaSuperUser, int? layerCod, int? elementoGraficoCod, int? batchSize, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            // Query per trovare tutti i record "da processare"
            stbQuery.AppendLine(" SELECT ");
            if (batchSize.HasValue && batchSize.Value > 0)
            {
                stbQuery.AppendLine(" TOP (@BatchSize) ");
                parametriSql.Add("@BatchSize", batchSize.Value);
            }
            stbQuery.AppendLine("PivaSuperUser, LayerElementoGrafico_Cod, ElementoGrafico_Cod");
            stbQuery.AppendLine(" FROM dbo.GIS_ElementiGrafici_Clustering");
            stbQuery.AppendLine(" WHERE Stato = 0"); 
            stbQuery.AppendLine("   AND PivaSuperUser = @PivaSuperUser");


            if (layerCod.HasValue && layerCod.Value > 0)
            {
                stbQuery.AppendLine("   AND LayerElementoGrafico_Cod = @LayerCod");
                parametriSql.Add("@LayerCod", layerCod.Value);
            }
            if (elementoGraficoCod.HasValue && elementoGraficoCod.Value > 0)
            {
                stbQuery.AppendLine("   AND ElementoGrafico_Cod = @ElementoGraficoCod");
                parametriSql.Add("@ElementoGraficoCod", elementoGraficoCod.Value);
            }

            stbQuery.AppendLine("   AND Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("   AND Validita_Fine >= @dtInizio");
            stbQuery.AppendLine("   AND inviato >= 0");

            parametriSql.Add("@PivaSuperUser", pivaSuperUser);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }
    }
}
