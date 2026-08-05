using System.Data;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.CodificheCAC.DAL.DataLayer;
using AgronicaNetCore.CodificheCAC.DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AgronicaNetCore.CodificheCAC.BIZ.Services
{
    public class CodificheCACService : BaseService, ICodificheCACService
    {
        private readonly IServiceProvider _serviceProvider;
        public CodificheCACService(IServiceProvider provider, IOptions<SecuritySettings> securitySettings) : base(provider)
        {
            _serviceProvider = provider;
        }

        public async Task<DataTable> GetAllCodificheAsync(AgronicaCoreParametriServer objParametriServer)
        {

            DataTable result;

            try
            {
                var codifiche = _serviceProvider.GetRequiredService<ICodificheCAC>();
                result = await codifiche.GetCodificaCACAsync(objParametriServer);
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        public async Task<bool> DeleteCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer)
        {

            bool result;

            try
            {
                var codifiche = _serviceProvider.GetRequiredService<ICodificheCAC>();
                result = await codifiche.DeleteCodificaCACAsync(objCAC, objParametriServer);
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        public async Task<bool> UpdateCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer)
        {
            bool result;

            try
            {
                var codifiche = _serviceProvider.GetRequiredService<ICodificheCAC>();
                result = await codifiche.UpdateCodificaCACAsync(objCAC, objParametriServer);
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        public async Task<bool> WriteCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer)
        {
            bool result;

            try
            {
                var codifiche = _serviceProvider.GetRequiredService<ICodificheCAC>();
                result = await codifiche.InsertCodificaCACAsync(objCAC, objParametriServer);
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}
