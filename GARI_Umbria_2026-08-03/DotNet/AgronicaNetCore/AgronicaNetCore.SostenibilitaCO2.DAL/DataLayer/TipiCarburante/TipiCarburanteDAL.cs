using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.TipiCarburante
{
    /// <summary>
    /// Implementazione dell'accesso alla tabella <c>Tipi_Carburante</c> tramite <c>AgronicaDataProvider6</c>.
    /// Utilizza query parametrizzate (no concatenazione SQL).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input <c>carburanti[].tipo_carburante</c>.
    /// </summary>
    public class TipiCarburanteDAL : BaseDALSostenibilitaCO2, ITipiCarburanteDAL
    {
        public TipiCarburanteDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) {}

        /// <inheritdoc/>
        public async Task<IReadOnlyList<TipoCarburanteEntity>> GetAllAsync(AgronicaCoreParametriServer objParametriServer)
        {
            const string sql = "SELECT Car_Cod, Car_Des FROM Carburanti ORDER BY Car_Des";

            DataTable? dt = null;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt == null || dt.Rows.Count == 0)
                return Array.Empty<TipoCarburanteEntity>();

            return dt.Rows.Cast<DataRow>()
                .Select(r => new TipoCarburanteEntity
                {
                    Car_Cod = Convert.ToInt32(r["Car_Cod"]),
                    Car_Des = r["Car_Des"]?.ToString() ?? string.Empty
                }).ToList().AsReadOnly();
        }
    }
}
