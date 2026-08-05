using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.LavorazioniAttivitaCDG;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.LavorazioniAttivitaCDG
{
    public class LavorazioniAttivitaCDG_APPService
        : BaseServiceAppBIZ,
            ILavorazioniAttivitaCDG_APPService
    {
        private readonly ILavorazioniAttivitaCDG_APP _lavorazioniAttivitaCdgApp;

        public LavorazioniAttivitaCDG_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _lavorazioniAttivitaCdgApp =
                _serviceProvider.GetRequiredService<ILavorazioniAttivitaCDG_APP>();
        }

        public async Task<List<LavorazioneAttivitaCDGEntity>> LeggiLavorazioniAttivitaCDGAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _lavorazioniAttivitaCdgApp.ReadAsync(objParametriServer);
                var result = new List<LavorazioneAttivitaCDGEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new LavorazioneAttivitaCDGEntity
                    {
                        lavorazioneId = row.Field<int?>("Lav_Cod") ?? 0,
                        attivitaCDGId = row.Field<int?>("Id_Attivita") ?? 0,
                    };

                    if (item.lavorazioneId <= 0)
                    {
                        continue;
                    }

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
