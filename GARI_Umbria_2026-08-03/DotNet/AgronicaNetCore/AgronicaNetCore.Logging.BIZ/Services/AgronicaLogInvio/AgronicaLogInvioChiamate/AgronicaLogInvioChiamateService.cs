using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioChiamate;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioChiamate
{
    public class AgronicaLogInvioChiamateService: BaseService, IAgronicaLogInvioChiamateService
    {

        private readonly IAgronicaLogInvioChiamate _agronicaLogInvioChiamateDAL;

        public AgronicaLogInvioChiamateService(IServiceProvider provider) : base(provider)
        {
            _agronicaLogInvioChiamateDAL = provider.GetRequiredService<IAgronicaLogInvioChiamate>();
        }

        public async Task<bool> WriteAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;
            try
            {

                result = await _agronicaLogInvioChiamateDAL.WriteAsync(writeAgronicaLogInvioChiamate, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }

        public async Task<bool> UpdateAsync(UpdateAgronicaLogInvioChiamate updateModel, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _agronicaLogInvioChiamateDAL.UpdateAsync(updateModel, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
