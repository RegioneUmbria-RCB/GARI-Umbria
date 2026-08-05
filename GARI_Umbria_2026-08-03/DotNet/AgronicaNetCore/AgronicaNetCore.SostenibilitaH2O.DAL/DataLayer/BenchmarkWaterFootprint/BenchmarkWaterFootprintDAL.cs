using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.BenchmarkWaterFootprint
{
    /// <summary>
    /// Implementazione del data access per la lettura del benchmark Green-Blue Water Footprint
    /// dalla tabella <c>waterfootprint</c> e per la trascodifica varietà GIAS → FAOSTAT
    /// tramite <c>CAC_Codifica_Veg_Cod</c>.
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint.
    /// </summary>
    public class BenchmarkWaterFootprintDAL : BaseDALSostenibilitaH2O, IBenchmarkWaterFootprintDAL
    {
        /// <summary>Valore di WF type atteso e sempre usato nella query.</summary>
        private const string WfTypeGreenBlue = "Green+Blue";

        /// <summary>Regione di fallback nazionale nella tabella benchmark.</summary>
        private const string RegioneCntryAverage = "CNTRY-average";

        /// <inheritdoc cref="BaseDALSostenibilitaH2O"/>
        public BenchmarkWaterFootprintDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<string?> GetIsoAlpha3FromAlpha2Async(
            string alpha2,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);
            if (string.IsNullOrWhiteSpace(alpha2))
                throw new ArgumentException("Il codice paese Alpha-2 non può essere vuoto.", nameof(alpha2));

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP 1 Codice_aLPHA_3");
            stbQuery.AppendLine("FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166");
            stbQuery.AppendLine("WHERE Codice = @alpha2");

            var sqlParams = new Dictionary<string, object>
            {
                ["@alpha2"] = alpha2
            };

            DataTable dt = await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), sqlParams);

            if (dt.Rows.Count == 0 || dt.Rows[0]["Codice_aLPHA_3"] == DBNull.Value)
                return null;

            return dt.Rows[0]["Codice_aLPHA_3"]?.ToString();
        }

        /// <inheritdoc/>
        public async Task<string?> GetProductCodeFaostatAsync(
            int vegCodGias,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP 1 veg_cod_coltiva");
            stbQuery.AppendLine("FROM CAC_Codifica_Veg_Cod");
            stbQuery.AppendLine("WHERE veg_cod_gias = @vegCodGias");

            var sqlParams = new Dictionary<string, object>
            {
                ["@vegCodGias"] = vegCodGias
            };

            DataTable dt = await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), sqlParams);

            if (dt.Rows.Count == 0 || dt.Rows[0]["veg_cod_coltiva"] == DBNull.Value)
                return null;

            return dt.Rows[0]["veg_cod_coltiva"]?.ToString();
        }

        /// <inheritdoc/>
        public async Task<decimal?> GetWaterFootprintAsync(
            string productCodeFaostat,
            string paeseIsoAlpha3,
            string? regionCode,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);
            if (string.IsNullOrWhiteSpace(productCodeFaostat))
                throw new ArgumentException("Il codice FAOSTAT non può essere vuoto.", nameof(productCodeFaostat));
            if (string.IsNullOrWhiteSpace(paeseIsoAlpha3))
                throw new ArgumentException("Il codice paese non può essere vuoto.", nameof(paeseIsoAlpha3));

            // ── Prima ricerca: con regione specifica (se fornita) ─────────────────
            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                var risultatoRegione = await EseguiQueryWaterFootprintAsync(
                    productCodeFaostat, paeseIsoAlpha3, regionCode, objParametriServer);

                if (risultatoRegione.HasValue)
                    return risultatoRegione;
            }

            // ── Fallback: CNTRY-average ───────────────────────────────────────────
            return await EseguiQueryWaterFootprintAsync(
                productCodeFaostat, paeseIsoAlpha3, RegioneCntryAverage, objParametriServer);
        }

        // ────────────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Esegue la query su <c>waterfootprint</c> per la combinazione
        /// (productCodeFaostat, paeseIsoAlpha3, regionCode, wf_type='Green+Blue').
        /// </summary>
        private async Task<decimal?> EseguiQueryWaterFootprintAsync(
            string productCodeFaostat,
            string paeseIsoAlpha3,
            string regionCode,
            AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP 1 Value");
            stbQuery.AppendLine("FROM waterfootprint");
            stbQuery.AppendLine("WHERE product_code_faostat = @productCodeFaostat");
            stbQuery.AppendLine("  AND Country = @paeseIsoAlpha3");
            stbQuery.AppendLine("  AND Region = @regionCode");
            stbQuery.AppendLine("  AND WF_type = @wfType");

            var sqlParams = new Dictionary<string, object>
            {
                ["@productCodeFaostat"] = productCodeFaostat,
                ["@paeseIsoAlpha3"]     = paeseIsoAlpha3,
                ["@regionCode"]         = regionCode,
                ["@wfType"]             = WfTypeGreenBlue
            };

            DataTable dt = await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), sqlParams);

            if (dt.Rows.Count == 0 || dt.Rows[0]["Value"] == DBNull.Value)
                return null;

            return Convert.ToDecimal(dt.Rows[0]["Value"]);
        }
    }
}
