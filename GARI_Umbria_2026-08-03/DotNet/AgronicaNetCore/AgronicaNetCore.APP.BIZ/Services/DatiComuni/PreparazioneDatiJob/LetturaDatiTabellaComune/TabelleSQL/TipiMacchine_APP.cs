using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class TipiMacchineApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.TipiMacchineApp _tipiMacchineApp;

        public TipiMacchineApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _tipiMacchineApp = _serviceProvider.GetRequiredService<DALMetaschema.TipiMacchineApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _tipiMacchineApp.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new TipoMacchinaEntity
                {
                    codice = r.Field<string>("codice"),
                    descrizione = r.Field<string>("descrizione"),
                })
                .ToList();
        }
    }
}

