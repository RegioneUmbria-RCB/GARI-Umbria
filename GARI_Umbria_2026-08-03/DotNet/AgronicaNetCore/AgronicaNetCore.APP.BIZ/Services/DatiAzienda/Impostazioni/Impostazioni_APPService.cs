using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impostazioni;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Impostazioni
{
    public class Impostazioni_APPService : BaseServiceAppBIZ, IImpostazioni_APPService
    {
        private readonly IImpostazioni_APP _impostazioniApp;

        public Impostazioni_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _impostazioniApp = _serviceProvider.GetRequiredService<IImpostazioni_APP>();
        }

        public async Task<List<ImpostazioneEntity>> LeggiImpostazioniAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _impostazioniApp.ReadAsync(piva, objParametriServer);
                var result = new List<ImpostazioneEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(
                        new ImpostazioneEntity
                        {
                            partitaIva = row.Field<string>("Piva") ?? piva,
                            centroAziendaleCod = row.Field<int?>("Sa_Cod") ?? 0,
                            codice = row.Field<int?>("Impostazione_Cod") ?? 0,
                            valore = row.Field<string>("Impostazione_Valore") ?? string.Empty,
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
