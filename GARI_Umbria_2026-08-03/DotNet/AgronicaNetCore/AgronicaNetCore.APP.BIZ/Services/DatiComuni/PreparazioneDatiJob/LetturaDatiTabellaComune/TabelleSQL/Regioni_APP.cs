using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.Localization;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.ISTAT;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.APP.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class RegioniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.RegioniApp _regioniAPP;

        public RegioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _regioniAPP = _serviceProvider.GetRequiredService<DALMetaschema.RegioniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _regioniAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new RegioneEntity
            {
                codice = r.Field<string?>("codice") ?? "",
                descrizione = r.Field<string?>("descrizione") ?? "",
                statoCod = r.Field<string?>("statoCod") ?? ""
            }).ToList();
        }
    }
}


