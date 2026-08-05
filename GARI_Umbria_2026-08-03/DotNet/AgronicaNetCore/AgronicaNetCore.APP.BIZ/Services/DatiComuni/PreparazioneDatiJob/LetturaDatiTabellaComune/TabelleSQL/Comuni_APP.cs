using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.ISTAT;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.APP.BIZ.Resources;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class ComuniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.ComuniApp _comuniAPP;

        public ComuniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _comuniAPP = _serviceProvider.GetRequiredService<DALMetaschema.ComuniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _comuniAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new ComuneEntity
            {
                codice = r.Field<string?>("codice") ?? "",
                descrizione = r.Field<string?>("descrizione") ?? "",
                cap = r.Field<string?>("cap") ?? "",
                provinciaCod = r.Field<string?>("provinciaCod") ?? "",
                statoCod = r.Field<string?>("statoCod") ?? ""
            }).ToList();
        }
    }
}


