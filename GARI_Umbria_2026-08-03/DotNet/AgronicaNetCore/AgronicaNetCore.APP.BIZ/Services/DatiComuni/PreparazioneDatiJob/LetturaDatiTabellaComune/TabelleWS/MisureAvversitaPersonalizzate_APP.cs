using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS
{
    public class MisureAvversitaPersonalizzateApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly MisureAvversitaApp _core;

        public MisureAvversitaPersonalizzateApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _core = _serviceProvider.GetRequiredService<MisureAvversitaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters) =>
            await _core.LeggiAsync(true, parameters);
    }
}

