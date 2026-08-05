using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaxCentriAziendali;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaxCentriAziendali
{
    public class AttivitaxCentriAziendali_APPService
        : BaseServiceAppBIZ,
            IAttivitaxCentriAziendali_APPService
    {
        private readonly IAttivitaxCentriAziendaliAPP _attivitaxCentriAziendaliApp;

        public AttivitaxCentriAziendali_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _attivitaxCentriAziendaliApp =
                _serviceProvider.GetRequiredService<IAttivitaxCentriAziendaliAPP>();
        }

        public async Task<
            List<CentroAziendaleAttivitaCDGEntity>
        > LeggiAttivitaxCentriAziendaliAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _attivitaxCentriAziendaliApp.ReadAsync(piva, objParametriServer);
                var result = new List<CentroAziendaleAttivitaCDGEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(
                        new CentroAziendaleAttivitaCDGEntity
                        {
                            partitaIva = row.Field<string>("Piva") ?? string.Empty,
                            centroAziendaleCod = row.Field<int>("Sa_Cod"),
                            attivitaCDGCod = row.Field<int>("ID_Attivita"),
                            inclusa = row.Field<short>("Inclusa"),
                        }
                    );
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
