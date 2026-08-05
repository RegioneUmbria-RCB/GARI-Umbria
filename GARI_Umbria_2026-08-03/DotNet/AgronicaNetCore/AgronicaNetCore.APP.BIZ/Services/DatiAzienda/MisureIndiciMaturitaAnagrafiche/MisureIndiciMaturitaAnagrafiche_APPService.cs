using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureIndiciMaturitaAnagrafiche;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureIndiciMaturitaAnagrafiche
{
    public class MisureIndiciMaturitaAnagrafiche_APPService : BaseServiceAppBIZ, IMisureIndiciMaturitaAnagrafiche_APPService
    {
        private readonly IMisureIndiciMaturitaAnagrafiche_APP _misureIndiciMaturitaAnagraficheApp;

        public MisureIndiciMaturitaAnagrafiche_APPService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _misureIndiciMaturitaAnagraficheApp = _serviceProvider.GetRequiredService<IMisureIndiciMaturitaAnagrafiche_APP>();
        }

        public async Task<List<MisuraIndiciMaturitaAnagraficheEntity>> LeggiMisureIndiciMaturitaAnagraficheAsync(AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var dt = await _misureIndiciMaturitaAnagraficheApp.ReadAsync(objParametriServer);
                var result = new List<MisuraIndiciMaturitaAnagraficheEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new MisuraIndiciMaturitaAnagraficheEntity
                    {
                        indiceMaturitaCod = row.Field<int?>("IND_MAT_COD") ?? 0,
                        udmCod = row.Field<int?>("UDM_COD") ?? 0,
                        descrizione = row.Field<string>("Anag_des") ?? string.Empty,
                        valore = row.Field<int?>("Anag_valore") ?? 0,
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