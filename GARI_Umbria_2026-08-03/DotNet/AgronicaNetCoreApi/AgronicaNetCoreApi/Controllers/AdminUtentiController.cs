using AgronicaCoreDTOStd.InData.Profilazione;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiProfiliPratiche;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
  /// <summary>
  /// Controller amministrativo per la gestione degli utenti.
  /// DS09-API – Endpoint Copia Visibilità Tra Utenti.
  /// </summary>
  [ApiController]
  [Authorize]
  [Route("v1/admin/utenti")]
  public class AdminUtentiController : BaseController
  {
    private readonly IUtentiProfiliPraticheService _utentiProfiliPraticheService;

    public AdminUtentiController(
        IServiceProvider provider,
        IOptions<SecuritySettings> securitySettings,
        IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
    {
      _utentiProfiliPraticheService = provider.GetRequiredService<IUtentiProfiliPraticheService>();
    }

    /// <summary>
    /// Copia la configurazione di visibilità (gerarchia e/o pratiche) da un utente sorgente
    /// a uno o più utenti target in modo atomico.
    /// DS09-API – POST /v1/admin/utenti/copia-visibilita.
    /// </summary>
    [HttpPost("copia-visibilita")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCopiaVisibilita([FromBody] CopiaVisibilitaObj inData)
    {
      RispostaStandard resp = new();

      // --- Fail-fast: validazione formato input ---
      if (string.IsNullOrWhiteSpace(inData.Template) || inData.Template.Length > 50)
      {
        resp.RispostaOK = false;
        resp.Errore = "idUtenteSorgente è obbligatorio e non può superare i 50 caratteri.";
        return BadRequest(resp);
      }

      if (!inData.CopyHierarchy && !inData.CopyProcedures)
      {
        resp.RispostaOK = false;
        resp.Errore = "Non è stata ordinata la copia di gerarchia né di pratiche. L'operazione è stata annullata.";
        return BadRequest(resp);
      }

      // Rimuove elementi vuoti e duplicati; l'auto-rimozione del template è demandata al servizio
      var cleanedTargets = (inData.Base ?? Enumerable.Empty<string>())
          .Where(t => !string.IsNullOrWhiteSpace(t))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToList();

      if (cleanedTargets.Count == 0)
      {
        resp.RispostaOK = false;
        resp.Errore = "idUtentiTarget non contiene utenti validi dopo la pulizia dell'input. L'operazione è stata annullata.";
        return BadRequest(resp);
      }

      try
      {
        var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
        if (!auth.status)
          return Unauthorized();

        var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

        await _utentiProfiliPraticheService.CopiaVisibilita(
            inData.Template,
            cleanedTargets,
            inData.CopyHierarchy,
            inData.CopyProcedures,
            objParametriDouble);

        resp.RispostaOK = true;
        return Ok(resp);
      }
      catch (ArgumentException ex)
      {
        resp.RispostaOK = false;
        resp.Errore = ex.Message;
        return BadRequest(resp);
      }
      catch (Exception ex)
      {
        resp.RispostaOK = false;
        resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        return StatusCode(StatusCodes.Status500InternalServerError, resp);
      }
    }
  }
}
