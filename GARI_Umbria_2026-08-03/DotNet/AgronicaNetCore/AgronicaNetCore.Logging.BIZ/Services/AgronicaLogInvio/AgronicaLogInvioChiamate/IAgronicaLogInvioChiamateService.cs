using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioChiamate
{
    public interface IAgronicaLogInvioChiamateService
    {
        Task<bool> WriteAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, AgronicaCoreParametriServer objParametriServer);

        Task<bool> UpdateAsync(UpdateAgronicaLogInvioChiamate updateModel, AgronicaCoreParametriServer objParametriServer);
    }
}
