using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPassword;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiPassword
{
    /// <summary>
    /// Servizio BIZ che espone la lettura dei dati password utente necessari al controllo scadenza.
    /// </summary>
    public class UtentiPasswordService : BaseServiceUtentiBIZ, IUtentiPasswordService
    {
        private readonly IUtentiPassword _utentiPasswordDAL;

        public UtentiPasswordService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _utentiPasswordDAL = _serviceProvider.GetRequiredService<IUtentiPassword>();
        }

        public async Task<(DateTime? DataUltimaModificaPassword, bool UtenteTrovato)> LeggiDataUltimaModificaPasswordAsync(
            string username,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            return await _utentiPasswordDAL.LeggiDataUltimaModificaPasswordAsync(username, objParametriUtenti);
        }
    }
}
