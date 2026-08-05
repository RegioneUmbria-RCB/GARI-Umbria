using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Servizi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Servizi
{
    public class ServiziService : BaseServiceMetaschemaBIZ, IServiziService
    {
        private readonly IServizi _serviziDAL;

        public ServiziService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _serviziDAL = _serviceProvider.GetRequiredService<IServizi>();
        }

        public async Task<DataTable> LeggiServiziEffettivamenteUsatiAsync(LeggiServizi_IN leggiServizi_IN, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _serviziDAL.LeggiServiziEffettivamenteUsatiAsync(leggiServizi_IN, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        public async Task<DataTable> LeggiServizi_StatiAsync(LeggiServiziStati_IN leggiServiziStati_IN, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _serviziDAL.LeggiServizi_StatiAsync(leggiServiziStati_IN, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiCatalogoServiziAsync(LeggiCatalogoServizi_IN leggiCatalogoServizi_IN, AgronicaCoreParametriServer objParametriServer)
        {
            if (leggiCatalogoServizi_IN.PageSize > 500)
                LogWarning(SanitizeLogMessage($"[CatalogoPratiche] page_size elevato: {leggiCatalogoServizi_IN.PageSize}"), objParametriServer);

            DataTable dt;
            try
            {
                dt = await _serviziDAL.LeggiCatalogoServiziAsync(leggiCatalogoServizi_IN, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            LogInformation(SanitizeLogMessage(
                $"[CatalogoPratiche] Recupero catalogo completato: {dt.Rows.Count} record. " +
                $"Filtri: filter='{leggiCatalogoServizi_IN.Filter}', includeExpired={leggiCatalogoServizi_IN.IncludeExpired}, page={leggiCatalogoServizi_IN.Page}, pageSize={leggiCatalogoServizi_IN.PageSize}"),
                objParametriServer);

            return dt;
        }
    }
}
