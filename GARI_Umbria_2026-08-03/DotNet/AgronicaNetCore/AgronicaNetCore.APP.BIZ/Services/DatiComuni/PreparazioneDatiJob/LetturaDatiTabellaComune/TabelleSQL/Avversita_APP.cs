using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class AvversitaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.AvversitaApp _avversitaApp;

        public AvversitaApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _avversitaApp = _serviceProvider.GetRequiredService<DALMetaschema.AvversitaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _avversitaApp.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new AvversitaEntity
                {
                    codice = r.Field<int>("codice"),
                    descrizione = r.Field<string>("descrizione"),
                    gruppoCod = r.Field<int>("gruppoCod"),
                })
                .ToList();
        }
    }
}

