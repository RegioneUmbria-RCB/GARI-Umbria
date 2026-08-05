using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.Zoo;
using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using AgronicaCoreModelsSTD.pianiDiCampionamento;
using InData.Zoo;
using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class ZooController : BaseController
    {
        public ZooController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("Stalle")]
        [ProducesResponseType(typeof(RispostaStandard<List<Stalla>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiStalle([FromBody] LeggiStalle request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Stalla>> result = new RispostaStandard<List<Stalla>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiStalle(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("Sottogruppi")]
        [ProducesResponseType(typeof(RispostaStandard<List<SottogruppoStalla>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiSottogruppiStalla([FromBody] LeggiSottogruppiStalla request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<SottogruppoStalla>> result = new RispostaStandard<List<SottogruppoStalla>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSottogruppiStalla(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("GiacenzeZoo")]
        [ProducesResponseType(typeof(RispostaStandard<List<GiacenzaZoo>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiGiacenzeZoo([FromBody] LeggiGiacenzeZoo request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
                
            RispostaStandard<List<GiacenzaZoo>> result = new RispostaStandard<List<GiacenzaZoo>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGiacenzeZoo(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }   

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("ScriviOperazioneZoo")]
        public ObjectResult ScriviOperazioneZoo(AgronicaCoreModelsSTD.attivita.Attivita attivita, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (attivita == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Attivita non valorizzata");
            }

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest((object)attivita);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviOperazioneZoo(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }
        
        [HttpPost]
        [Route("LeggiPianiCampionamento")]
        [ProducesResponseType(typeof(RispostaStandard<List<PianoDiCampionamento>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPianiCampionamento([FromBody] LeggiPianiCampionamento request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<PianoDiCampionamento>> result = new RispostaStandard<List<PianoDiCampionamento>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPianiCampionamento(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("ScriviPianoCampionamento")]
        public ObjectResult ScriviPianoCampionamento(PianoDiCampionamento pianoCampionamento, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (pianoCampionamento == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Piano campionamento non valorizzato");
            }

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest((object)pianoCampionamento);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviPianoCampionamento(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("ScriviPianoCampionamentoConSpostamento")]
        public ObjectResult ScriviPianoCampionamentoConSpostamento(PianoDiCampionamentoSpostamento pianoCampionamento, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (pianoCampionamento == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Piano campionamento non valorizzato");
            }

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest((object)pianoCampionamento);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviPianoCampionamentoConSpostamento(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpGet]
        [Route("AnomalieZoo")]
        public ObjectResult LeggiAnomalieZoo([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescr>> result = new RispostaStandard<List<BaseCodeDescr>>();

            try
            {
                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                var r = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAnomalieZoo(request);                
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = JsonConvert.DeserializeObject<List<BaseCodeDescr>>(r.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("ScriviCapoAnimale")]
        public ObjectResult ScriviCapoAnimale(CapoAnimale capo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (capo == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Capo animale non valorizzato");
            }

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest((object)capo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCapoAnimale(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("SpostamentoGruppi")]
        public ObjectResult SpostamentoGruppi(SpostamentoGruppi spostamentoGruppi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (spostamentoGruppi == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Gruppi non valorizzati");
            }

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(spostamentoGruppi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SpostamentoGruppi(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }
    }
}
