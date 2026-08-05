using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.DAL.ProviderMappings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Machineries
{
    public class MachineService : BaseServiceSmartTractorBIZ, IMachineService
    {
        private readonly ILogger<MachineService> _logger;
        private readonly IProviderMappingMachine _machineDAL;

        public MachineService(ILogger<MachineService> logger, IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _logger = logger;
        }

        public async Task<string> GetProviderCodeAsync(AgronicaCoreParametriServer serverParams, int macCod)
        {
            string res = "";
            DataTable? result = null;
            try
            {
                result = await _machineDAL.GetMachineMappingAsync(macCod, serverParams);

                if (result != null && result.Rows.Count > 0)
                {
                    res = result.Rows[0]["ProviderCod"].ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, serverParams, ex);
            }

            return res;
        }
    }
}
