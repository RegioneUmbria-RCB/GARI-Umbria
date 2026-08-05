using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS
{
    public class SpecieVegetaliStadiCrescitaPersonalizzatiApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly FasiFenologicheApp _core;

        public SpecieVegetaliStadiCrescitaPersonalizzatiApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _core = _serviceProvider.GetRequiredService<FasiFenologicheApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
            => await _core.LeggiAsync(true, parameters);
    }
}


