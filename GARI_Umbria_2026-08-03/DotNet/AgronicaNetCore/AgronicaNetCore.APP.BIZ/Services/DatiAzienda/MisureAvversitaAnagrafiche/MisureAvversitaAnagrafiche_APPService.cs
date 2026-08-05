using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureAvversitaAnagrafiche;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureAvversitaAnagrafiche
{
    public class MisureAvversitaAnagrafiche_APPService
        : BaseServiceAppBIZ,
            IMisureAvversitaAnagrafiche_APPService
    {
        private readonly IMisureAvversitaAnagrafiche_APP _misureAvversitaAnagraficheApp;

        public MisureAvversitaAnagrafiche_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _misureAvversitaAnagraficheApp =
                _serviceProvider.GetRequiredService<IMisureAvversitaAnagrafiche_APP>();
        }

        public async Task<
            List<MisuraAvversitaAnagraficheEntity>
        > LeggiMisureAvversitaAnagraficheAsync(AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var dt = await _misureAvversitaAnagraficheApp.ReadAsync(objParametriServer);
                var result = new List<MisuraAvversitaAnagraficheEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new MisuraAvversitaAnagraficheEntity
                    {
                        codice = row.Field<int?>("anag_cod") ?? 0,
                        descrizione = row.Field<string>("anag_des") ?? string.Empty,
                        valore = (int)(row.Field<double?>("anag_valore") ?? 0),
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
