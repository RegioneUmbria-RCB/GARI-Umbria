using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Progetti;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Progetti
{
    public class Progetti_APPService : BaseServiceAppBIZ, IProgetti_APPService
    {
        private readonly IProgetti_APP _progettiApp;

        public Progetti_APPService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _progettiApp = _serviceProvider.GetRequiredService<IProgetti_APP>();
        }

        public async Task<List<ProgettoEntity>> LeggiProgettiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _progettiApp.ReadAsync(piva, objParametriServer);
                var result = new List<ProgettoEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new ProgettoEntity
                    {
                        partitaIva = row.Field<string>("piva") ?? piva,
                        codice = (long)row.Field<int>("Imputazione_Cod"),
                        descrizione = row.Field<string>("Imputazione_Nome") ?? string.Empty,
                        attivitaCDGCod = row.Field<int?>("Id_Attivita") ?? 0,
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
