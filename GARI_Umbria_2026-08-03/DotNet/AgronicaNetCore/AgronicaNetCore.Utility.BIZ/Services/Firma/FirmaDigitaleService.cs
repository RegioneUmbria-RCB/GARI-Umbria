using AgronicaCoreModelsSTD.AgronicaChatGPT;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utility.BIZ.Resources;
using AgronicaNetCore.Utility.BIZ.Services.Firma.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma
{
    public class FirmaDigitaleService : BaseService, IFirmaDigitaleService
    {

        private readonly ISecurityLayerDAL _securityLayer;
        private readonly IFirmaProviderFactory _providerFactory;
        private readonly string? _encryptKey;

        public FirmaDigitaleService(
            IServiceProvider provider, 
            IConfiguration config,
            IFirmaProviderFactory providerFactory) : base(provider)
        {
            _securityLayer = provider.GetRequiredService<ISecurityLayerDAL>();
            _encryptKey = config.GetValue<string>("cr2");
            _providerFactory = providerFactory;
        }

        private async Task<ConfigFirmaDigitale?> LoadConfiguration(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string configEncrypted = await _securityLayer.LeggiConfigurazioneSitiScalareAsync("ConfigFirmaDigitale", objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(configEncrypted))
            {
                LogWarning("ConfigFirmaDigitale non configurata su Configurazione_Siti");
                return null;
            }

            //in configString avremo una stringa cifrata, in quanto saranno presenti password e altri dati sensibili
            //leggiamo quindi le keys di cifratura 
            string cr = await _securityLayer.LeggiConfigurazioneSitiScalareAsync("cr", objParametriServer, objParametriSuperServer);

            string configString;

            if (string.IsNullOrWhiteSpace(_encryptKey))
                configString = Base.Utility.Security.DecryptString(configEncrypted, cr);
            else
                configString = Base.Utility.Security.DecryptString(configEncrypted, cr, _encryptKey);

            ConfigFirmaDigitale? configFirmaDigitale;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                configFirmaDigitale = JsonSerializer.Deserialize<ConfigFirmaDigitale>(configString, options);

                if (configFirmaDigitale is null)
                {
                    LogWarning("Deserializzazione in errore");
                    return null;
                }
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Errore deserializzazione. {MESSAGE}", ex.Message);
                throw;
            }

            return configFirmaDigitale;

        }

        public async Task<byte[]?> SignAsync(string jsonContent, bool timeStampLevelEnabled, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {

            var config = await LoadConfiguration(objParametriServer, objParametriSuperServer);

            if (config is null)
                throw new Exception("Chiave di configurazione Firma Digitale non trovata");

            var provider = _providerFactory.Get(config.Provider ?? "");

            return await provider.SignAsync(jsonContent, timeStampLevelEnabled, config);
        }
    }
}
