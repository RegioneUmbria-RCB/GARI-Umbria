using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.APP.BIZ.Resources;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class GruppiAvversitaAttiveApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.GruppiAvversitaAttiveApp _gruppiAvversitaAttiveAPP;

        public GruppiAvversitaAttiveApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _gruppiAvversitaAttiveAPP = _serviceProvider.GetRequiredService<DALMetaschema.GruppiAvversitaAttiveApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _gruppiAvversitaAttiveAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new GruppoAvversitaAttiveEntity
            {
                codice = r.Field<int>("codice"),
                descrizione = r.Field<string>("descrizione")
            }).ToList();
        }
    }
}


