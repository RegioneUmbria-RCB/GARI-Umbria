using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieZootecniche;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class SpecieZootecnicheApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.SpecieZootecnicheApp _specieZootecnicheAPP;

        public SpecieZootecnicheApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _specieZootecnicheAPP =
                _serviceProvider.GetRequiredService<DALMetaschema.SpecieZootecnicheApp>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _specieZootecnicheAPP.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new SpecieZootecnicaEntity
                {
                    genereCod = r.Field<int>("genereCod"),
                    specieCod = r.Field<int>("specieCod"),
                    indirizzoProdCod = r.Field<int>("indirizzoProdCod"),
                    descrizione = r.Field<string>("descrizione"),
                })
                .ToList();
        }
    }
}

