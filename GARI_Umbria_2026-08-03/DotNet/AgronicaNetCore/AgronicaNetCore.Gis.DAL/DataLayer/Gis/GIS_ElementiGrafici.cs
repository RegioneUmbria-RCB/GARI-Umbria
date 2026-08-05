using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaNetCore.Base.Constants;
using Exception = System.Exception;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    /// <summary>
    /// Recupera la geometria WKT e la proiezione EPSG di un appezzamento da
    /// <c>GIS_ElementiGrafici</c> + <c>GIS_Entita</c>.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regola 8 (WKT, EPSG).
    /// </summary>
    public class GIS_ElementiGrafici : BaseDALGis, IGIS_ElementiGrafici
    {
        public GIS_ElementiGrafici(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<(string? Wkt, string? Epsg)> GetPoligonoConEpsgAsync(
            string piva,
            int appezzaCode,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita IVA.", nameof(piva));

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) g.Poligono_GeoEntity_WKT AS Poligono_WKT,");
            stbQuery.AppendLine("   '4326' AS SRID");
            stbQuery.AppendLine("FROM Appezzamento a WITH(NOLOCK)");
            stbQuery.AppendLine("JOIN GIS_Entita e(NOLOCK)");
            stbQuery.AppendLine("ON a.PIVA = e.Piva");
            stbQuery.AppendLine("AND a.APPEZZA = e.Appezza");
            stbQuery.AppendLine("JOIN GIS_ElementiGrafici g(NOLOCK)");
            stbQuery.AppendLine("ON e.Entita_Cod = g.Entita_Cod");
            stbQuery.AppendLine("WHERE  a.Piva = @piva");
            stbQuery.AppendLine("AND  a.Appezza = @appezza");
            stbQuery.AppendLine("AND  g.Poligono_GeoEntity IS NOT NULL");

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"]    = piva,
                ["@appezza"] = appezzaCode
            };

            DataTable dt;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt.Rows.Count == 0)
                return (null, null);

            var wkt  = dt.Rows[0]["Poligono_WKT"]?.ToString();
            var srid = dt.Rows[0]["SRID"]?.ToString();

            return (
                string.IsNullOrWhiteSpace(wkt)  ? null : wkt,
                string.IsNullOrWhiteSpace(srid) ? null : srid
            );
        }
    }
}
