using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.UtilityDB.BIZ.Resources;

namespace AgronicaNetCore.UtilityDB.BIZ.Services.UtilityDB
{
    public class UtilityDBService : BaseServiceUtilityDBBiz, IUtilityDBService
    {
        private readonly IUtilityDB _utilityDBDAL;

        public UtilityDBService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utilityDBDAL = _serviceProvider.GetRequiredService<IUtilityDB>();
        }

        [Obsolete("Usare il metodo ReadSQLVersionMajorAsync")]
        public async Task<int> Read_SQL_Version_YearAsync(AgronicaCoreParametri objParametri)
        {
            int version;
            try
            {
                version = await _utilityDBDAL.Read_SQL_Version_YearAsync(objParametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return version;
        }

        public async Task<int> Read_SQL_Compatibility_LevelAsync(AgronicaCoreParametri objParametri)
        {
            int compatibilityLevel;
            try
            {
                compatibilityLevel = await _utilityDBDAL.Read_SQL_Compatibility_LevelAsync(objParametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return compatibilityLevel;
        }

        public Task<string> GetDBName(AgronicaCoreParametri objParametri)
        {
            string DBName;
            try
            {
                DBName = _utilityDBDAL.GetDBName(objParametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return Task.FromResult(DBName);
        }

        public async Task<int> ReadSQLVersionMajorAsync(AgronicaCoreParametri objParametri)
        {
            int major;
            try
            {
                major = await _utilityDBDAL.ReadSQLVersionMajorAsync(objParametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return major;
        }
    }
}
