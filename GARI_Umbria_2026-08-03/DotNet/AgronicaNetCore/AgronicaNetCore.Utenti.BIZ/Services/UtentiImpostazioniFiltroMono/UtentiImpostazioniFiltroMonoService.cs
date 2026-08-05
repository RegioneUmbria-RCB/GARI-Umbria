using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiImpostazioniFiltroMono
{
    public class UtentiImpostazioniFiltroMonoService : BaseServiceUtentiBIZ, IUtentiImpostazioniFiltroMonoService
    {
        private readonly IUtentiImpostazioniFiltroMono _utentiImpostazioniFiltroMonoDAL;

        public UtentiImpostazioniFiltroMonoService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiImpostazioniFiltroMonoDAL = _serviceProvider.GetRequiredService<IUtentiImpostazioniFiltroMono>();
        }

        public async Task<DataTable> LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti Impostazione_Cod, int ID_0, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;

            try
            {
                dt = await _utentiImpostazioniFiltroMonoDAL.LeggiAsync(Impostazione_Cod, ID_0, objParametriUtenti, objParametriServer);
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
