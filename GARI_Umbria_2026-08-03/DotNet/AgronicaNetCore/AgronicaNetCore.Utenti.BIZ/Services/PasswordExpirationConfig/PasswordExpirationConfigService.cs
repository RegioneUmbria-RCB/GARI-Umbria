using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Exceptions;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationConfig
{
    /// <summary>
    /// Servizio BIZ che legge i parametri di configurazione della scadenza password.
    /// Se PASSWORD_EXPIRATION_DAYS e' assente o non valido restituisce null, quindi la feature resta disabilitata.
    /// </summary>
    public class PasswordExpirationConfigService : BaseServiceUtentiBIZ, IPasswordExpirationConfigService
    {
        private const string ChiaveParametro = "PASSWORD_EXPIRATION_DAYS";
        private const string ChiavePreavviso = "GIORNI_PREAVVISO_SCADENZA_PASSWORD";

        private readonly ISecurityLayerDAL _securityLayerDAL;

        public PasswordExpirationConfigService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDAL = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        public async Task<int?> LeggiParametroScadenzaPasswordAsync(
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);

            DataTable result;
            try
            {
                result = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(ChiaveParametro, objParametriServer);
            }
            catch (Exception ex)
            {
                throw new ConfigurazioneDALException(
                    $"Errore durante la lettura del parametro '{ChiaveParametro}' dalla tabella Configurazione_Siti.", ex);
            }

            DataRow? row = result.Rows
                .Cast<DataRow>()
                .FirstOrDefault(r =>
                    r["Sito_Cod"] is not DBNull &&
                    Convert.ToInt32(r["Sito_Cod"]) == 0);

            if (row is null)
            {
                LogInformation(
                    $"Il parametro '{ChiaveParametro}' non e' presente in Configurazione_Siti per Sito_Cod=0. " +
                    "Controllo scadenza password disabilitato per questo sito.",
                    objParametriServer);
                return null;
            }

            string? valoreStringa = row["Valore"]?.ToString();

            if (!int.TryParse(valoreStringa, out int giorni) || giorni <= 0)
            {
                LogInformation(
                    $"Il valore '{valoreStringa}' del parametro '{ChiaveParametro}' non e' un intero positivo valido. " +
                    "Controllo scadenza password disabilitato per questo sito.",
                    objParametriServer);
                return null;
            }

            return giorni;
        }

        public async Task<int?> LeggiParametroGiorniPreavvisoAsync(
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);

            DataTable result;
            try
            {
                result = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(ChiavePreavviso, objParametriServer);
            }
            catch (Exception ex)
            {
                throw new ConfigurazioneDALException(
                    $"Errore durante la lettura del parametro '{ChiavePreavviso}' dalla tabella Configurazione_Siti.", ex);
            }

            DataRow? row = result.Rows
                .Cast<DataRow>()
                .FirstOrDefault(r =>
                    r["Sito_Cod"] is not DBNull &&
                    Convert.ToInt32(r["Sito_Cod"]) == 0);

            if (row is null)
            {
                LogInformation(
                    $"Il parametro '{ChiavePreavviso}' non e' presente in Configurazione_Siti per Sito_Cod=0. " +
                    "Notifiche di preavviso disabilitate per questo sito.",
                    objParametriServer);
                return null;
            }

            string? valoreStringa = row["Valore"]?.ToString();

            if (!int.TryParse(valoreStringa, out int giorni) || giorni <= 0)
            {
                LogInformation(
                    $"Il valore '{valoreStringa}' del parametro '{ChiavePreavviso}' non e' un intero positivo valido. " +
                    "Notifiche di preavviso disabilitate per questo sito.",
                    objParametriServer);
                return null;
            }

            return giorni;
        }
    }
}
