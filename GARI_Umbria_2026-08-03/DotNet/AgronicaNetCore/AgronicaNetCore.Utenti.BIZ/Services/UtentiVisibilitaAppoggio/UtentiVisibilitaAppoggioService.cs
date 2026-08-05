using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiVisibilitaAppoggio
{
    public class UtentiVisibilitaAppoggioService : BaseServiceUtentiBIZ, IUtentiVisibilitaAppoggioService
    {
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggioDAL;

        public UtentiVisibilitaAppoggioService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiVisibilitaAppoggioDAL = _serviceProvider.GetRequiredService<IUtentiVisibilitaAppoggio>();
        }

        public async Task<DataTable?> ReadAsync(int entity, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;

            try
            {
                res = await _utentiVisibilitaAppoggioDAL.ReadAsync(entity, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return res;

        }     

    }
}
