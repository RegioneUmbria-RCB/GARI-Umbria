using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAgenda
{
    public interface IAgronicaLogInvioAgenda
    {
        public Task<bool> WriteAsync(WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> Estrai_Agende_PerInvioSistemiEsterniAsync(
               AgronicaCoreParametriServer objParametriServer,
               int tipoEsportazione, string? tipo = null, 
               IEnumerable<string>? pivaToFilter = null, IEnumerable<int>? lavcodToFilter = null, IEnumerable<int>? vegcodToFilter = null,
               IEnumerable<int>? typeOperationToFilter = null, IEnumerable<string>? filtroEsiti = null, IntervalloTemporale? dateInterval = null
            );
        Task<bool> CheckAgendaInviataAsync(int idAgenda, int tipoEsportazione, AgronicaCoreParametriServer objParametriServer);
    }
}
