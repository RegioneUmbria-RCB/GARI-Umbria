using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.APP.BIZ.Resources;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class SpecieVegetaliApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.SpecieVegetaliAPP _specieVegetaliAPP;

        public SpecieVegetaliApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _specieVegetaliAPP = _serviceProvider.GetRequiredService<DALMetaschema.SpecieVegetaliAPP>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _specieVegetaliAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new SpecieEntity
            {
                codice = r.Field<int>("codice"),
                descrizione = r.Field<string>("descrizione")
            }).ToList();
        }
    }
}


