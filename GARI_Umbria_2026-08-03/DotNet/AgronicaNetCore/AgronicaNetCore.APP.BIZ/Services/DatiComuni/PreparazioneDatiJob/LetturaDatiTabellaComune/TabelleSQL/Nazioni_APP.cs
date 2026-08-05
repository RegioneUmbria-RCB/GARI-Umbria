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
    public class NazioniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {

        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.NazioniApp _nazioniAPP;

        public NazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _nazioniAPP = _serviceProvider.GetRequiredService<DALMetaschema.NazioniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _nazioniAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new NazioneEntity
            {
                codice = r.Field<string?>("codice") ?? "",
                descrizione = r.Field<string?>("descrizione") ?? ""
            }).ToList();
        }
    }
}


