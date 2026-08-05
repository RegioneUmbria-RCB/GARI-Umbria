using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioChiamate
{
    public interface IAgronicaLogInvioChiamate
    {
        public Task<bool> WriteAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> UpdateAsync(UpdateAgronicaLogInvioChiamate updateModel, AgronicaCoreParametriServer objParametriServer);
    }
}
