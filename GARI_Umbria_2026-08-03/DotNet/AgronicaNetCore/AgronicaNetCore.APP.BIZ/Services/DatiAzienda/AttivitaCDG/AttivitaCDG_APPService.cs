using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaCDG;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaCDG
{
    public class AttivitaCDG_APPService : BaseServiceAppBIZ, IAttivitaCDG_APPService
    {
        private readonly IAttivitaCDG_APP _attivitaCdgApp;

        public AttivitaCDG_APPService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _attivitaCdgApp = _serviceProvider.GetRequiredService<IAttivitaCDG_APP>();
        }

        public async Task<List<AttivitaCDGEntity>> LeggiAttivitaCDGAsync(AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var dt = await _attivitaCdgApp.ReadAsync(objParametriServer);
                var result = new List<AttivitaCDGEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new AttivitaCDGEntity
                    {
                        codice = row.Field<int?>("Id_Attivita") ?? 0,
                        descrizione = row.Field<string>("Desc") ?? string.Empty,
                        classType = "AttivitaCDG",
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
