using AgronicaNetCore.APP.BIZ.Common;
using Microsoft.Extensions.Options;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune
{
    public class LetturaDatiTabellaComuneAPPService
        : BaseServiceAppBIZ,
            ILetturaDatiTabellaComuneAPPService
    {
        private readonly LetturaDatiTabellaComuneAppFactory _factory;
        private readonly SincroWeb2AppSettings _settings;

        public LetturaDatiTabellaComuneAPPService(
            IServiceProvider provider,
            LetturaDatiTabellaComuneAppFactory factory,
            IStringLocalizer<Messages> localizer,
            IOptions<SincroWeb2AppSettings> settings
        )
            : base(provider, localizer)
        {
            _factory = factory;
            _settings = settings.Value;
        }

        public async Task<object> LeggiAsync(
            TabellaComune tabella,
            LetturaTabellaComuneAppParameters parameters
        )
        {
            var handler = _factory.Create(tabella);

            for (int attempt = 1; attempt <= _settings.LetturaTabellaMaxRetry; attempt++)
            {
                try
                {
                    return await handler.LeggiAsync(parameters);
                }
                catch (Exception ex) when (attempt < _settings.LetturaTabellaMaxRetry)
                {
                    LogWarning(
                        $"Tentativo {attempt} fallito per {tabella}, ritentando tra {_settings.LetturaTabellaRetryDelaySeconds * attempt} secondi",
                        null,
                        ex
                    );
                    await Task.Delay(TimeSpan.FromSeconds(_settings.LetturaTabellaRetryDelaySeconds * attempt));
                }
                catch (Exception ex)
                {
                    LogError($"Lettura {tabella} fallita dopo {_settings.LetturaTabellaMaxRetry} tentativi", null, ex);
                    throw new MaxRetryExceededException(
                        $"Lettura {tabella} non riuscita dopo {_settings.LetturaTabellaMaxRetry} tentativi",
                        ex
                    );
                }
            }

            throw new MaxRetryExceededException($"Impossibile completare la lettura di {tabella}");
        }
    }
}

