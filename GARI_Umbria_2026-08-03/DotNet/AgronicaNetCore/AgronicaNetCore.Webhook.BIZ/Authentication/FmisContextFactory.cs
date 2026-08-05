using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Webhook.DAL.DataLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgronicaNetCore.Webhook.BIZ.Authentication
{
    public class FmisContextFactory : IFmisContextFactory
    {
        private readonly IConfiguration _config;
        private readonly ILogger<FmisContextFactory> _logger;

        public FmisContextFactory(IConfiguration config, ILogger<FmisContextFactory> logger)
        {
            _config = config;
            _logger = logger;
        }

        public FmisContextDataBase? CreateFmisContextData(string fmisContextHeader)
        {
            try
            {
                var cr2 = _config.GetValue<string>("cr2");
                var decrypted = Security.DecryptString(fmisContextHeader, cr2);
                var baseData = JsonConvert.DeserializeObject<FmisContextDataBase>(decrypted);

                switch (baseData?.Tipo)
                {
                    case WebhookTipo.Base:
                        return baseData;
                    case WebhookTipo.EsitoConsiglioNutrizione:
                        return JsonConvert.DeserializeObject<FmisContextDataNutrizione>(decrypted);
                    default:
                        _logger.LogWarning("Tipo di fmis-context non riconosciuto: {Type}", baseData?.Tipo);
                        return null;
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Impossibile decifrare fmis-context header");
                return null;
            }
        }
    }
}
