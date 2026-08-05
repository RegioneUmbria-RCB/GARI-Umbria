using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using DALRapportiContabili = AgronicaNetCore.MetaSchema.DAL.DataLayer.RapportiContabili;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class RapportiContabiliApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALRapportiContabili.RapportiContabiliApp _rapportiContabiliApp;

        public RapportiContabiliApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _rapportiContabiliApp = _serviceProvider.GetRequiredService<DALRapportiContabili.RapportiContabiliApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _rapportiContabiliApp.LeggiAsync(parameters);

            return dt.AsEnumerable()
                .Select(r => new RapportiContabiliEntity
                {
                    codice = ToInt32OrDefault(r, "codice"),
                    descrizione = r.Field<string>("descrizione") ?? string.Empty,
                    cliente = ToInt32OrDefault(r, "cliente") == 1,
                    fornitore = ToInt32OrDefault(r, "fornitore") == 1,
                    dipendente = ToInt32OrDefault(r, "dipendente") == 1,
                    terzista = ToInt32OrDefault(r, "terzista") == 1,
                    legale = ToInt32OrDefault(r, "legale") == 1,
                    agente = ToInt32OrDefault(r, "agente") == 1,
                    consulente = ToInt32OrDefault(r, "consulente") == 1,
                })
                .ToList();
        }

        private static int ToInt32OrDefault(DataRow row, string columnName)
        {
            var value = row[columnName];
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }
    }
}
