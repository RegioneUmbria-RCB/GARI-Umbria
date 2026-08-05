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
    public class GruppiAvversitaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.GruppiAvversitaApp _gruppiAvversitaAPP;

        public GruppiAvversitaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _gruppiAvversitaAPP = _serviceProvider.GetRequiredService<DALMetaschema.GruppiAvversitaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _gruppiAvversitaAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new GruppoAvversitaEntity
            {
                codice = r.Field<int>("codice"),
                descrizione = r.Field<string>("descrizione")
            }).ToList();
        }
    }
}


