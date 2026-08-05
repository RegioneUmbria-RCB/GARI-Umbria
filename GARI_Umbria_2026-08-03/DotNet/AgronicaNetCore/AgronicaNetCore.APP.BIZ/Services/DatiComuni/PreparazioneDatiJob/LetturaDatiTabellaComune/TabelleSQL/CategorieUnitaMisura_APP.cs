using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using DALMetaschema = AgronicaNetCore.MetaSchema.DAL.DataLayer.CategorieUnitaMisura;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleSQL
{
    public class CategorieUnitaMisuraApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly DALMetaschema.CategorieUnitaMisuraAPP _categorieUnitaMisuraAPP;

        public CategorieUnitaMisuraApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _categorieUnitaMisuraAPP =
                _serviceProvider.GetRequiredService<DALMetaschema.CategorieUnitaMisuraAPP>();
        }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var dt = (DataTable)await _categorieUnitaMisuraAPP.LeggiAsync(parameters);
            return dt.AsEnumerable()
                .Select(r => new CategorieUnitaMisuraEntity
                {
                    codice = r.Field<int>("codice"),
                    elemCod = -1,
                    nomeComune = "",
                    descrizione = r.Field<string>("descrizione"),
                    simbolo = r.Field<string>("simbolo"),
                    tipoControlloCod = r.Field<int>("tipoControllo")
                })
                .ToList();
        }
    }
}

