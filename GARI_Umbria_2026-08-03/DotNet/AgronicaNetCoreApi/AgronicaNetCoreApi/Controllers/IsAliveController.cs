using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    // IsAlive serve solo per vedere che il sito sia su, non necessita di autorizzazione, quindi non mettiamo [Authorize]
    public class IsAliveController : BaseController
    {
        private readonly ILogger<IsAliveController> _logger;

        public IsAliveController(ILogger<IsAliveController> logger,  IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer) 
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("IsAlive")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsAlive(string InData)
        {
            RispostaStandard resp = new();

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "GiasVersioneCorrente.txt");
            if (System.IO.File.Exists(filePath))
            {
                string fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                resp.RispostaStringa = fileContent;
                resp.RispostaOK = true;
                return Ok(resp);
            }
            else
            {
                resp.RispostaStringa = "";
                return NotFound(resp);
            }
        }
    }
}
