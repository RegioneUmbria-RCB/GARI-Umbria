using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.Finalita;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.APP.BIZ.Resources;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class FinalitaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.FinalitaApp _finalitaAPP;

        public FinalitaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _finalitaAPP = _serviceProvider.GetRequiredService<DALMetaschema.FinalitaApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _finalitaAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new GruppoFinalitaEntity
            {
                codice = r.Field<int>("codice"),
                descrizione = r.Field<string>("descrizione"),
                specieCod = r.Field<int>("specieCod")
            }).ToList();
        }
    }
}


