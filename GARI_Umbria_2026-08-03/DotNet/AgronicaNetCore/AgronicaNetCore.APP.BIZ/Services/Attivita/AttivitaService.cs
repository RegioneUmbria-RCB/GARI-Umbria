using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using InData.Agenda;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.APP.BIZ.Services.Attivita
{
    public class AttivitaService : BaseServiceAppBIZ, IAttivitaService
    {
        private readonly ChiamaCoreWS _coreWsClient;

        public AttivitaService(IServiceProvider provider, IStringLocalizer<Messages> localizer, ChiamaCoreWS coreWsClient) : base(provider, localizer)
        {
            _coreWsClient = coreWsClient;
        }

        public async Task<ArchivioAttivitaCampagna> LeggiAgendeEBrogliacciPerAppAsync_OLD(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, AgronicaCoreParametriTriple tripleParams, string bearerToken, string urlCoreWs)
        {
            return await LeggiAgendeBrogliacciInternalAsync(piva, dataRiferimento, dataUltimaSincro, soloImpiantiAttivi, tripleParams, bearerToken, urlCoreWs, false);
        }

        public async Task<ArchivioAttivitaCampagna> LeggiBrogliacciPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, AgronicaCoreParametriTriple tripleParams, string bearerToken, string urlCoreWs)
        {
            return await LeggiAgendeBrogliacciInternalAsync(piva, dataRiferimento, dataUltimaSincro, soloImpiantiAttivi, tripleParams, bearerToken, urlCoreWs, true);
        }

        private async Task<ArchivioAttivitaCampagna> LeggiAgendeBrogliacciInternalAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, AgronicaCoreParametriTriple tripleParams, string bearerToken, string urlCoreWs, bool leggiSoloBrogliacci)
        {
            ArchivioAttivitaCampagna result;

            var url = urlCoreWs + "/Agenda/Agenda.asmx/LeggiAgendeEBrogliacciPerApp";

            var leggiAgenda = new LeggiAgenda(piva, dataRiferimento, dataUltimaSincro, soloImpiantiAttivi, leggiSoloBrogliacci);
            try
            {
                var json = await _coreWsClient.ChiamaCoreWSAsync(url, leggiAgenda, tripleParams, bearerToken);

                if (string.IsNullOrEmpty(json))
                    throw new Exception("Risposta nulla da web service");

                JObject jObj = JObject.Parse(json);
                JToken d = jObj["d"];

                var risposta = JsonConvert.DeserializeObject<ArchivioAttivitaCampagna>(d.ToString());
                if (risposta is null)
                    throw new DeserializationException("Something went wrong");

                result = risposta;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, tripleParams.ObjParametriServer, ex);
                throw;
            }
            return result;
        }


    }
}
