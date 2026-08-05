using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class OperazioniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.OperazioniApp _operazioniApp;

        public OperazioniApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _operazioniApp = _serviceProvider.GetRequiredService<DALMetaschema.OperazioniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _operazioniApp.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new LavorazioneEntity
                {
                    codice = r.Field<int>("codice"),
                    descrizione = r.Field<string>("descrizione"),
                    classType = "Lavorazione"
                })
                .ToList();
        }
    }
}

