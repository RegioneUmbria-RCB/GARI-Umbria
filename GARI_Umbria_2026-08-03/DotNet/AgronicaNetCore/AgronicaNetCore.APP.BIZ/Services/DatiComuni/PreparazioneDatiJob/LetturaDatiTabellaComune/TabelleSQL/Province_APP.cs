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
    public class ProvinceApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.ProvinceApp _provinceAPP;

        public ProvinceApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _provinceAPP = _serviceProvider.GetRequiredService<DALMetaschema.ProvinceApp>();
        }
        
        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _provinceAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new ProvinciaEntity
            {
                codice = r.Field<string?>("codice") ?? "",
                descrizione = r.Field<string?>("descrizione") ?? "",
                sigla = r.Field<string?>("sigla") ?? "",
                regioneCod = r.Field<string?>("regioneCod") ?? "",
                regioneDes = r.Field<string?>("regioneDes") ?? "",
                statoCod = r.Field<string?>("statoCod") ?? ""
            }).ToList();
        }
    }
}


