using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.OperazioniCausali
{
    public class OperazioniCausaliAPP_Service : BaseServiceAppBIZ, IOperazioniCausaliAPP_Service
    {
        private readonly ILoggingService _loggingService;
        private readonly IOperazioniCausali_APP _operazioniCausaliAPP;

        public OperazioniCausaliAPP_Service(
            IServiceProvider provider,
            IOperazioniCausali_APP operazioniCausali,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _operazioniCausaliAPP = operazioniCausali;
        }

        public async Task<List<OperazioneCausaleEntity>> LeggiAsync(
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var dt = (DataTable)await _operazioniCausaliAPP.LeggiAsync(objParametriServer);
            return dt.AsEnumerable()
                .Select(r => new OperazioneCausaleEntity
                {
                    id = r.Field<int>("id"),
                    causale = r.Field<string>("causale"),
                    lavCod = r.Field<int>("lavCod"),
                    Validita_Inizio = r.Field<DateTime>("validitaInizio"),
                    Validita_Fine = r.Field<DateTime>("validitaFine"),
                })
                .ToList();
        }
    }
}

