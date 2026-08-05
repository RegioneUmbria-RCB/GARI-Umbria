using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Squadre;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Squadre
{
    /// <summary>
    /// Ref: DS08-BL sections 3.2 and 7.2.
    /// Orchestration service for Squadre acquisition in the DatiAzienda flow.
    /// </summary>
    public class Squadre_APPService : BaseServiceAppBIZ, ISquadre_APPService
    {
        private readonly ISquadre_APP _squadreApp;

        public Squadre_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _squadreApp = _serviceProvider.GetRequiredService<ISquadre_APP>();
        }

        /// <summary>
        /// Ref: DS08-BL sections 3.2.3 and 7.2.
        /// Validates the request and delegates the team extraction to the DAL.
        /// </summary>
        public async Task<List<SquadreEntity>> LeggiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita iva.", nameof(piva));

            ArgumentNullException.ThrowIfNull(objParametriServer);
            ArgumentNullException.ThrowIfNull(objParametriUtenti);

            try
            {
                return await _squadreApp.LeggiAsync(piva, objParametriServer, objParametriUtenti);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}