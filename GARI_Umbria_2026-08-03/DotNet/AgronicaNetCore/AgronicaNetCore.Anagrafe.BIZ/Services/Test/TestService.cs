using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaCoreNet6.BusinessLayer.Services.Test
{
    public class TestService : BaseServiceAnagrafeBIZ, ITestService
    {
        private readonly ILoggingService _loggingService;

        public TestService(IServiceProvider serviceProvider, IStringLocalizer<Messages> localizer) : base(serviceProvider, localizer) { 
            _loggingService = serviceProvider.GetRequiredService<ILoggingService>();
        }        

        public (string,int,DateTime) GetPippo()
        {
            return new("ciao", 1, DateTime.Now);
        }
        
    }
}
