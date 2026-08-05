using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Machineries
{
    public interface IMachineService
    {
        Task<string> GetProviderCodeAsync(AgronicaCoreParametriServer serverParams, int macCod);
    }
}
