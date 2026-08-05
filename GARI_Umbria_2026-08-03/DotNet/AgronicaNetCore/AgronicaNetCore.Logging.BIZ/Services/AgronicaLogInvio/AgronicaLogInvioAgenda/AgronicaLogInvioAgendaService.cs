using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioChiamate;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAgenda;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioAgenda
{
    public class AgronicaLogInvioAgendaService : BaseService, IAgronicaLogInvioAgendaService
    {

        private readonly IAgronicaLogInvioAgenda _agronicaLogInvioAgendaDAL;
        private readonly IAgronicaLogInvioChiamateService _agronicaLogInvioChiamateBIZ;

        public AgronicaLogInvioAgendaService(IServiceProvider provider) : base(provider)
        {
            _agronicaLogInvioAgendaDAL = provider.GetRequiredService<IAgronicaLogInvioAgenda>();
            _agronicaLogInvioChiamateBIZ = provider.GetRequiredService<IAgronicaLogInvioChiamateService>();
        }

        public async Task<bool> WriteAsync(WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;
            try
            {
                result = await _agronicaLogInvioAgendaDAL.WriteAsync(writeAgronicaLogInvioAgenda,objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }

        public async Task<bool> WriteAlsoInvioChiamateAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;
            try
            {
                await OpenConnectionAsync(objParametriServer);

                result = await _agronicaLogInvioChiamateBIZ.WriteAsync(writeAgronicaLogInvioChiamate, objParametriServer);

                if(result && writeAgronicaLogInvioChiamate.ID > 0)
                {
                    writeAgronicaLogInvioAgenda.ID_Log_Invio = writeAgronicaLogInvioChiamate.ID;
                    result = await _agronicaLogInvioAgendaDAL.WriteAsync(writeAgronicaLogInvioAgenda, objParametriServer);
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
