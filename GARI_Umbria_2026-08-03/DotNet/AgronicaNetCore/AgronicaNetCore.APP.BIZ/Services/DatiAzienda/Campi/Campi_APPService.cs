using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Campi;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Campi
{
    public class Campi_APPService : BaseServiceAppBIZ, ICampi_APPService
    {
        private readonly ICampi_APP _campiApp;

        public Campi_APPService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _campiApp = _serviceProvider.GetRequiredService<ICampi_APP>();
        }

        public async Task<List<CampoEntity>> LeggiCampiAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var dt = await _campiApp.ReadAsync(piva, objParametriServer);
                var result = new List<CampoEntity>();
                // var keys = new HashSet<string>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new CampoEntity
                    {
                        partitaIva = row.Field<string>("Piva") ?? string.Empty,
                        centroAziendaleCod = row.Field<int?>("sa_cod") ?? 0,
                        codice = row.Field<int?>("campo_cod") ?? 0,
                        descrizione = row.Field<string>("campo_des") ?? string.Empty,
                    };

                    // var key = $"{item.partitaIva}|{item.centroAziendaleCod}|{item.codice}";
                    // if (keys.Add(key))
                    // {
                    //     result.Add(item);
                    // }
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
