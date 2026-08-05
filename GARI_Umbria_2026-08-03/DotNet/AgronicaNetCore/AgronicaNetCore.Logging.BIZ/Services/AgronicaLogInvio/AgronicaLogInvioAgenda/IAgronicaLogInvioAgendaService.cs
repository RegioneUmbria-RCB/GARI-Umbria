using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioAgenda
{
    public interface IAgronicaLogInvioAgendaService
    {
        Task<bool> WriteAsync(WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda ,AgronicaCoreParametriServer objParametriServer);

        Task<bool> WriteAlsoInvioChiamateAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer);
    }
}
