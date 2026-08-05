using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using AgronicaNetCore.Base.Base;
using DALImpiantiIrrigazioni = AgronicaNetCore.MetaSchema.DAL.DataLayer.ImpiantiIrrigazioni;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class ImpiantiIrrigazioniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALImpiantiIrrigazioni.ImpiantiIrrigazioniApp _impiantiIrrigazioniApp;

        public ImpiantiIrrigazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _impiantiIrrigazioniApp = _serviceProvider.GetRequiredService<DALImpiantiIrrigazioni.ImpiantiIrrigazioniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _impiantiIrrigazioniApp.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new ImpiantoIrrigazioneEntity
                {
                    codice = r.Field<int>("codice"),
                    descrizione = r.Field<string>("descrizione")
                })
                .ToList();
        }
    }
}
