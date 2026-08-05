using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.APP.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class AvversitaSpecieApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.AvversitaSpecieApp _avversitaSpecieAPP;

        public AvversitaSpecieApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _avversitaSpecieAPP = _serviceProvider.GetRequiredService<DALMetaschema.AvversitaSpecieApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _avversitaSpecieAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new AvversitaSpecieEntity
            {
                specieCod = r.Field<int>("specieCod"),
                avversitaCod = r.Field<int>("avversitaCod")
            }).ToList();
        }
    }
}


