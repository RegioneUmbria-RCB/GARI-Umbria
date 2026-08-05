using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodificaProdotti;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CodificaProdotti
{
    public class CodificaProdotti_APPService : BaseServiceAppBIZ, ICodificaProdotti_APPService
    {
        private readonly ICodificaProdotti_APP _codificaProdottiApp;

        public CodificaProdotti_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _codificaProdottiApp = _serviceProvider.GetRequiredService<ICodificaProdotti_APP>();
        }

        public async Task<List<CodificaProdottoEntity>> LeggiCodificaProdottiAsync(
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _codificaProdottiApp.ReadAsync(
                    "",
                    Enum_Tipo_CAC_Codifica_ProdottiAziendali.NessunFiltro,
                    ELEM_COD.SEMENTI,
                    objParametriServer
                );
                var result = new List<CodificaProdottoEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new CodificaProdottoEntity
                    {
                        elemCod = row.Field<int?>("Elem_Cod") ?? 0,
                        codice = row.Field<int?>("Codice_GIAS") ?? 0,
                        descrizione = row.Field<string>("Desc_GIAS") ?? string.Empty,
                        codProdotto = row.Field<string>("Cod_Prodotto_Cliente") ?? string.Empty,
                        descProdotto = row.Field<string>("Desc_Prodotto_Cliente") ?? string.Empty,
                        catProdotto =
                            row.Field<string>("Categoria_Prodotto_Cliente") ?? string.Empty,
                        partitaIva = row.Field<string>("Piva") ?? string.Empty,
                        codArticolo = row.Field<string>("Cod_Articolo") ?? string.Empty,
                        tipoCodifica = row.Field<int?>("Tipo_Codifica") ?? 0,
                    };

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
