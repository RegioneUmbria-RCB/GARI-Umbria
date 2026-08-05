using AgronicaCoreDTOStd.InData.ActivityImport;
using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Apezzamenti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAgenda;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioChiamate;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.ActivityImport.Mapper;
using AgronicaNetCore.Operazione.BIZ.Services.Agenda;
using AgronicaNetCore.Operazione.BIZ.Services.Agenda.Mapper;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agronica_Log_Agenda;
using InData.ActivityImport;
using InData.Agenda;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OutData.ActivityImport;
using System;
using System.Runtime.InteropServices;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ActivityImport
{
    public class ActivityImportService : BaseServiceOperazioneBIZ, IActivityImportService
    {
        private readonly IActivityImportMapper<IActivityImportData> _mapperOrogel;
        private readonly IAttivitaToAgenda _agendaMapper;
        private readonly IAgendaService _agenda;
        private readonly Utility.BIZ.Services.AgroZip.IAgroZipService _agroZip;
        private readonly IImpresa _imprese;
        private readonly IAppezzamenti _appezzamenti;
        private readonly IAgronicaLogInvioChiamate _logInvioChiamate;
        private readonly IAgronicaLogInvioAgenda _logInvioAgenda;
        private readonly IAgronica_Log_Agenda _logAgenda;

        public ActivityImportService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _mapperOrogel = _serviceProvider.GetRequiredService<IActivityImportMapper<IActivityImportData>>();
            _agendaMapper = _serviceProvider.GetRequiredService<IAttivitaToAgenda>();
            _agroZip = _serviceProvider.GetRequiredService<Utility.BIZ.Services.AgroZip.IAgroZipService>();
            _imprese = _serviceProvider.GetRequiredService<IImpresa>();
            _agenda = _serviceProvider.GetRequiredService<IAgendaService>();
            _appezzamenti = _serviceProvider.GetRequiredService<IAppezzamenti>();
            _logInvioChiamate = _serviceProvider.GetRequiredService<IAgronicaLogInvioChiamate>();
            _logInvioAgenda = _serviceProvider.GetRequiredService<IAgronicaLogInvioAgenda>();
            _logAgenda = _serviceProvider.GetRequiredService<IAgronica_Log_Agenda>();
        }

        private string IsValidRequest(ActivityImportJsonObject importJson)
        {
            if (importJson == null)
                return "JSON non valorizzato";
            if (importJson.cuaa == "")
                return "CUAA non valorizzato";
            if (importJson.dati == "")
                return "Dati operazione non valorizzati";
            return "";
        }

        private IActivityImportMapper<IActivityImportData> GetMapper(enum_Esportazioni_Sistema_Cod sysCod)
        {
            return sysCod switch
            {
                enum_Esportazioni_Sistema_Cod.OnPlantImport_Conferimenti => _mapperOrogel,
                _ => throw new NotImplementedException()
            };
        }

        private async Task BloccaAppezzamenti(IEnumerable<Appezzamento.PK> appezzamenti, AgronicaCoreParametriServer objParametriServer, DateTime blockDate, bool useTransaction = true)
        {
            try
            {
                await OpenConnectionAsync(objParametriServer);
                foreach (var item in appezzamenti)
                {
                    await _appezzamenti.BloccaSbloccaAsync(true, item, objParametriServer, blockDate);
                }
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriServer, true);
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }
        }

        private async Task<bool> WriteInvioLog(
            bool isSuccessful,
            ActivityImportJsonObject importJson,
            int idAgenda,
            OperazioneAgenda agenda,
            AgronicaCoreParametriServer objParametriServer,
            string errorMsg = ""
        )
        {
            WriteAgronicaLogInvioChiamate iCall = new();
            WriteAgronicaLogInvioAgenda iAgenda = new(_ID_Agenda: idAgenda);

            importJson.dati = _agroZip.UnZipBase64(importJson.dati);
            JsonSerializerSettings tzh = new JsonSerializerSettings();
            tzh.DateFormatString = "yyyy-MM-ddT00:00:00Z";
            string datiAttivita = JsonConvert.SerializeObject(importJson, tzh);
            iCall.Dati_Inviati = datiAttivita;
            iCall.Dati_Ricevuti = errorMsg;
            iCall.Tipo_Operazione = TipoOperazioneStr.Insert;
            iCall.Esito = isSuccessful ? EsitoStr.OK : EsitoStr.KO;
            iCall.Tipo_Esportazione = (int)enum_Esportazioni_Sistema_Cod.OnPlantImport_Conferimenti;
            iCall.Data_Invio = DateTime.Now;

            IActivityImportData? operationData = JsonConvert.DeserializeObject<ActivityImportData>(importJson.dati);
            iAgenda.Chiave_Esterna = operationData?.codice;
            iAgenda.Tipo_Esportazione = (int)enum_Esportazioni_Sistema_Cod.OnPlantImport_Conferimenti;
            iAgenda.ID_Log_Invio = 0;
            iAgenda.Piva = agenda.Piva;
            iAgenda.Chiave = agenda.Piva + "_" + idAgenda;

            return await WriteAgronicaLogInvioChiamateAgendaAsync(iCall, iAgenda, objParametriServer);
        }

        public async Task<ActivityImportResult> ImportExternalActivity(
            ActivityImportJsonObject importJson,
            enum_Esportazioni_Sistema_Cod sysCod,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            ActivityImportResult result = new ActivityImportResult();
            result.errore = IsValidRequest(importJson);
            if (result.errore != "")
            {
                return result;
            }

            int noRaccoglitore = 0;
            string piva = await _imprese.PivaFromCuaaAsync(importJson.cuaa, objParametriServer);
            string dataJson = _agroZip.UnZipBase64(importJson.dati);
            IActivityImportData? operationData = JsonConvert.DeserializeObject<ActivityImportData>(dataJson);

            var mapper = GetMapper(sysCod);
            mapper.SetBase(operationData!);
            var activity = await mapper.MapActivity(piva, noRaccoglitore, objParametriServer);
            var agenda = await _agendaMapper.MapAttivitaToAgenda(activity, objParametriServer);
            int idAgenda = 0;
            // Non esegue check DPI perché non è un trattamento

            try
            {
                await OpenConnectionAsync(objParametriServer);

                idAgenda = await _agenda.ScriviAttivitaAgendaAsync((activity, agenda), objParametriServer, false);
                if (operationData.flag_blocco_impianti)
                {
                    IEnumerable<Appezzamento.PK> app = activity.centriDiCosto.Cast<EsercizioCDC>()
                        .Select(cdc => cdc.esercizio.impiantoPK.appezzamentoPK);
                    await BloccaAppezzamenti(app, objParametriServer, operationData.data, false);
                }
                await WriteInvioLog(true, importJson, idAgenda, agenda, objParametriServer);
                result.message = "OK";
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriServer, true);
                await WriteInvioLog(false, importJson, idAgenda, agenda, objParametriServer, ex.Message);
                result.errore = ex.Message;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }
            return result;
        }

        private async Task<bool> WriteAgronicaLogInvioChiamateAgendaAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;
            try
            {
                await OpenConnectionAsync(objParametriServer);

                result = await _logInvioChiamate.WriteAsync(writeAgronicaLogInvioChiamate, objParametriServer);

                if (result && writeAgronicaLogInvioChiamate.ID > 0)
                {
                    writeAgronicaLogInvioAgenda.ID_Log_Invio = writeAgronicaLogInvioChiamate.ID;
                    result = await _logInvioAgenda.WriteAsync(writeAgronicaLogInvioAgenda, objParametriServer);
                }

            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriServer, true);
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }

            return result;

        }
    }
}
