using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.ParcoMacchine
{
    public class ParcoMacchineService : BaseServiceAnagrafeBIZ, IParcoMacchineService
    {
        private readonly ILoggingService _loggingService;

        public ParcoMacchineService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _loggingService = provider.GetRequiredService<ILoggingService>();
        }

        public Task<AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine?> ReadMachineAsync(AgronicaCoreParametriServer objParametriServer, string piva)
        {
            throw new NotImplementedException();
        }
    }
}
