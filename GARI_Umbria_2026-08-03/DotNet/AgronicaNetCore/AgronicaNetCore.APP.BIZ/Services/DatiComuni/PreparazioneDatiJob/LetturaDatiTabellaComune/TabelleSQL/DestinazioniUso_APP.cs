using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class DestinazioniUsoApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly ICodiciAnagrafe _codiciAnagrafeDal;

        public DestinazioniUsoApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _codiciAnagrafeDal = _serviceProvider.GetRequiredService<ICodiciAnagrafe>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = await _codiciAnagrafeDal.LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(parameters.ObjParametriTriple.ObjParametriServer);
            return dt.AsEnumerable().Select(r => new DestinazioneUsoEntity
            {
                codice = r.Field<int>("Codice"),
                descrizione = r.Field<string>("Descrizione")
            }).ToList();
        }
    }
}


