using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti;
using AgronicaNetCore.APP.BIZ.Resources;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class InfestantiAttiveApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly DALMetaschema.InfestantiAttiveApp _infestantiAttiveAPP;
        private readonly ILoggingService _loggingService;

        public InfestantiAttiveApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _infestantiAttiveAPP = _serviceProvider.GetRequiredService<DALMetaschema.InfestantiAttiveApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _infestantiAttiveAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new InfestantiAttiveEntity
            {
                codice = r.Field<int>("codice"),
                gruppo = r.Field<int>("gruppo"),
                descrizione = r.Field<string>("descrizione")
            }).ToList();
        }
    }
}
 

