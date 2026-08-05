using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.Varieta;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class VarietaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.VarietaApp _varietaApp;

        public VarietaApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _varietaApp = _serviceProvider.GetRequiredService<DALMetaschema.VarietaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _varietaApp.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new VarietaEntity
                {
                    codice = r.Field<int>("codice"),
                    descrizione = r.Field<string>("descrizione"),
                    specieCod = r.Field<int>("specieCod"),
                })
                .ToList();
        }
    }
}

