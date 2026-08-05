using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.ParcoMacchine
{
    public interface IParcoMacchineService
    {
        Task<AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine?> ReadMachineAsync(AgronicaCoreParametriServer objParametriServer, string piva);
    }
}
