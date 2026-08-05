using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS
{
    public class IndiciMaturitaSpecieVegetaliApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly IndiciMaturitaApp _core;

        public IndiciMaturitaSpecieVegetaliApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _core = _serviceProvider.GetRequiredService<IndiciMaturitaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var (_, _, specieVegetali) = await _core.GetOrFetchAsync(false, parameters);
            return specieVegetali;
        }
    }
}

