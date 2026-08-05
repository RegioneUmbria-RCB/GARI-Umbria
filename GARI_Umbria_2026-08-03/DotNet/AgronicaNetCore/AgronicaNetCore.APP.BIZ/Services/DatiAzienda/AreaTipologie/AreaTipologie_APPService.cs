using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.AreaTipologie;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AreaTipologie
{
    public class AreaTipologie_APPService : BaseServiceAppBIZ, IAreaTipologie_APPService
    {
        private readonly IAreaTipologie_APP _areaTipologieApp;

        public AreaTipologie_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _areaTipologieApp = _serviceProvider.GetRequiredService<IAreaTipologie_APP>();
        }

        public async Task<(
            List<AreaTipologieDocumentoEntity> aree,
            List<TipologiaDocumentoEntity> tipologie
        )> LeggiTipologieAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var dt = await _areaTipologieApp.ReadAsync(objParametriServer);
            var aree = new List<AreaTipologieDocumentoEntity>();
            var tipologie = new List<TipologiaDocumentoEntity>();

            foreach (DataRow row in dt.Rows)
            {
                var area = new AreaTipologieDocumentoEntity
                {
                    codice = row.Field<int?>("ID_Area") ?? 0,
                    descrizione = row.Field<string>("Nome_Area") ?? string.Empty,
                }; //todo forse va messo un groupby

                if (area.codice != 0)
                    aree.Add(area);

                var tipologia = new TipologiaDocumentoEntity
                {
                    codice = row.Field<int?>("ID_Tipologia") ?? 0,
                    descrizione = row.Field<string>("Nome_Tipologia") ?? string.Empty,
                    areaCod = row.Field<int?>("ID_Area") ?? 0,
                };

                if (tipologia.codice != 0)
                    tipologie.Add(tipologia);
            }

            return (aree, tipologie);
        }
    }
}
