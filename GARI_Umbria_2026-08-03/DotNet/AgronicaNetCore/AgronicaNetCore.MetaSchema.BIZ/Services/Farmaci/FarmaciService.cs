using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Farmaci;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Farmaci
{
    public class FarmaciService : BaseServiceMetaschemaBIZ, IFarmaciService
    {
        private readonly IFarmaci _farmaciDAL;

        public FarmaciService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _farmaciDAL = _serviceProvider.GetRequiredService<IFarmaci>();
        }

        public async Task<DataTable> LeggiFarmaciAsync(int Farm_Cod, string Aic, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;

            try
            {
                dt = await _farmaciDAL.LeggiFarmaciAsync(Farm_Cod, Aic, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }
    }
}
