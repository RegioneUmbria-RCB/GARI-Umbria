using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.Localization;
using System.Data;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.APP.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class OperazioniCombinazioniApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {  
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.OperazioniCombinazioniApp _operazioniCombinazioniAPP;

        public OperazioniCombinazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _operazioniCombinazioniAPP = _serviceProvider.GetRequiredService<DALMetaschema.OperazioniCombinazioniApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _operazioniCombinazioniAPP.LeggiAsync(parameters);
            return dt.AsEnumerable().Select(r => new OperazioniCombinazioniEntity
            {
                codice = r.Field<int>("codice"),
                lavCod1 = r.Field<int>("lavCod1"),
                lavCod2 = r.Field<int>("lavCod2"),
                lavCod3 = r.Field<int>("lavCod3"),
                lavCod4 = r.Field<int>("lavCod4"),
                lavCod5 = r.Field<int>("lavCod5")
            }).ToList();
        }
    }
}


