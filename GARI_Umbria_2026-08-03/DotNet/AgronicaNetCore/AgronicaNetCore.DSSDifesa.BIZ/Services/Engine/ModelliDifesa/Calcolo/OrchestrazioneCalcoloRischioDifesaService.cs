using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.BIZ.Resources;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Acquisizione;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Acquisizione;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.ModelliDifesa.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Business orchestrator service implementing DS07-BL_ Orchestrazione Calcolo Rischio Completa.
    /// Coordina il flusso end-to-end: meteo, infestanti, calcolo modelli con timeout e contesto.
    /// </summary>
    public class OrchestrazioneCalcoloRischioDifesaService : BaseDSSDifesaBIZService, IOrchestrazioneCalcoloRischioDifesaService
    {
        private const int GlobalTimeoutSeconds = 120;
        //private const int MeteoTimeoutMilliseconds = 500;
        //private const int InfestantiTimeoutMilliseconds = 2000;

        private readonly IAcquisizioneMeteoIbridaService _meteoService;
        private readonly IAcquisizioneModelliDifesaService _modelliService;
        private readonly ICalcoloRischioPerModelliDifesaService _calcoloService;

        public OrchestrazioneCalcoloRischioDifesaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _meteoService = provider.GetRequiredService<IAcquisizioneMeteoIbridaService>();
            _modelliService = provider.GetRequiredService<IAcquisizioneModelliDifesaService>();
            _calcoloService = provider.GetRequiredService<ICalcoloRischioPerModelliDifesaService>();
        }

        /// <inheritdoc />
        public async Task<OrchestrazioneCalcoloRischioDifesaResponse> CalcolaRischioAvversitaCompletaAsync(
            OrchestrazioneCalcoloRischioDifesaRequest input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            ValidateInput(input);

            using var globalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            globalCts.CancelAfter(TimeSpan.FromSeconds(GlobalTimeoutSeconds));

            var response = new OrchestrazioneCalcoloRischioDifesaResponse
            {
                TimestampRaccolta = DateTimeOffset.UtcNow.ToString("O"),
                TimeoutGlobaleRaggiunto = false
            };

            try
            {
                var meteo = await AcquireMeteoAsync(input, objParametriServer, objParametriSuperServer, globalCts.Token);
                var modelli = await AcquireModelliAsync(input, objParametriServer, objParametriSuperServer, globalCts.Token);

                var calcoloRequest = new CalcoloRischioPerModelliDifesaRequest
                {
                    LivelloDettaglio = input.LivelloDettaglio,
                    MeteoData = meteo.Select(m => new MeteoDataPointInput
                    {
                        dataOra = m.DataOra.ToString("O"),
                        temp = m.Temp,
                        prec = m.Prec,
                        relHum = m.RelHum,
                        lw = m.Lw
                    }).ToList(),
                    Modelli = modelli.Select(m => new ModelloDifesaInput
                    {
                        Id = m.Id,
                        Code = m.Code,
                        Description = m.Description
                    }).ToList()
                };

                var calcoloResult = await _calcoloService.CalcolaRischioPerModelliDifesaAsync(
                    calcoloRequest,
                    objParametriServer,
                    objParametriSuperServer,
                    globalCts.Token);

                // If downstream hits global timeout, propagate with partial results
                response.TimeoutGlobaleRaggiunto = calcoloResult.TimeoutGlobaleRaggiunto;
                response.RisultatiModelli = calcoloResult.RisultatiModelli ?? new List<ModelloRisultatoOutput>();

                if (response.TimeoutGlobaleRaggiunto)
                {
                    response.RisultatiModelli = response.RisultatiModelli.Select(r =>
                    {
                        if (!string.Equals(r.Status, "COMPLETATO", StringComparison.OrdinalIgnoreCase))
                        {
                            r.Status = "TIMEOUT_GLOBALE";
                            if (string.IsNullOrWhiteSpace(r.MessaggioErrore))
                                r.MessaggioErrore = "Global timeout reached during model execution.";
                        }
                        return r;
                    }).ToList();
                }

                return response;
            }
            catch (OperationCanceledException) when (globalCts.IsCancellationRequested)
            {
                response.TimeoutGlobaleRaggiunto = true;
                response.RisultatiModelli = response.RisultatiModelli ?? new List<ModelloRisultatoOutput>();
                return response;
            }
            catch (CalcoloRischioDifesaRequestValidationException)
            {
                throw;
            }
            catch (CalcoloRischioDifesaDataReadFataleException)
            {
                throw;
            }
            catch (CalcoloRischioDifesaDataReadTimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CalcoloRischioDifesaDataReadFataleException("Errore fatale durante l'orchestrazione del calcolo rischio avversità.", ex);
            }
        }

        private static void ValidateInput(OrchestrazioneCalcoloRischioDifesaRequest input)
        {
            if (input == null)
                throw new CalcoloRischioDifesaRequestValidationException("Input non può essere null.");

            if (string.IsNullOrWhiteSpace(input.CropCode))
                throw new CalcoloRischioDifesaRequestValidationException("CropCode è obbligatorio.");

            if (string.IsNullOrWhiteSpace(input.DataInizio))
                throw new CalcoloRischioDifesaRequestValidationException("DataInizio è obbligatorio.");

            if (string.IsNullOrWhiteSpace(input.DataFine))
                throw new CalcoloRischioDifesaRequestValidationException("DataFine è obbligatorio.");

            DateTimeOffset dataInizio;
            if (!DateTimeOffset.TryParse(input.DataInizio, out dataInizio))
                throw new CalcoloRischioDifesaRequestValidationException("DataInizio non è una data ISO 8601 valida.");

            DateTimeOffset dataFine;
            if (!DateTimeOffset.TryParse(input.DataFine, out dataFine))
                throw new CalcoloRischioDifesaRequestValidationException("DataFine non è una data ISO 8601 valida.");

            if (dataInizio >= dataFine)
                throw new CalcoloRischioDifesaRequestValidationException("DataFine deve essere maggiore di DataInizio.");

            bool hasStation = !string.IsNullOrWhiteSpace(input.StationCod);
            bool hasCoordinates = input.Coordinates != null && input.Coordinates.IsValid();
            if (!hasStation && !hasCoordinates)
                throw new CalcoloRischioDifesaRequestValidationException("Deve essere fornito StationCod oppure coordinates valide.");

            if (input.ModelliCodici != null && input.ModelliCodici.Any(code => string.IsNullOrWhiteSpace(code)))
                throw new CalcoloRischioDifesaRequestValidationException("ModelliCodici non può contenere elementi vuoti.");
        }

        private async Task<List<MeteoDataH>> AcquireMeteoAsync(
            OrchestrazioneCalcoloRischioDifesaRequest input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken globalCancellationToken)
        {
            //using var cts = CancellationTokenSource.CreateLinkedTokenSource(globalCancellationToken);
            //cts.CancelAfter(TimeSpan.FromMilliseconds(MeteoTimeoutMilliseconds));

            //try
            //{
                var requestMeteo = new AcquisizioneMeteoIbridaRequest
                {
                    StationCod = input.StationCod,
                    Coordinates = input.Coordinates,
                    DataInizio = input.DataInizio,
                    DataFine = input.DataFine
                };

            var me = await _meteoService.AcquisisciDatiMeteorologiciAsync(
                requestMeteo,
                objParametriServer,
                objParametriSuperServer,
                globalCancellationToken);
                    //cts.Token);

                if (me == null || me.MeteoData == null || me.MeteoData.Count == 0 || string.Equals(me.Status, "ERRORE", StringComparison.OrdinalIgnoreCase))
                    throw new CalcoloRischioDifesaDataReadFataleException("Impossibile recuperare dati meteo validi dal servizio meteo.");

                return me.MeteoData;
            //}
            //catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
            //{
            //    throw new CalcoloRischioDifesaDataReadTimeoutException("Timeout fase acquisizione meteo raggiunto.", ex);
            //}
        }

        private async Task<List<ModelloDifesa>> AcquireModelliAsync(
            OrchestrazioneCalcoloRischioDifesaRequest input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken globalCancellationToken)
        {
            //using var cts = CancellationTokenSource.CreateLinkedTokenSource(globalCancellationToken);
            //cts.CancelAfter(TimeSpan.FromMilliseconds(InfestantiTimeoutMilliseconds));

            //try
            //{
                var requestModelli = new AcquisizioneModelliDSSDifesaRequest
                {
                    CropCode = input.CropCode,
                    VarCode = input.VarCode,
                    ModelliCodici = input.ModelliCodici
                };

            var modelliResponse = await _modelliService.RecuperaListaModelliDifesaDinamicaAsync(
                requestModelli,
                objParametriServer,
                objParametriSuperServer,
                globalCancellationToken);
                    //cts.Token);

                if (modelliResponse == null || modelliResponse.Modelli == null || modelliResponse.Modelli.Count == 0 || string.Equals(modelliResponse.Status, "ERRORE", StringComparison.OrdinalIgnoreCase))
                    throw new CalcoloRischioDifesaDataReadFataleException("Impossibile recuperare la lista infestanti dal servizio modelli.");

                return modelliResponse.Modelli;
            //}
            //catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
            //{
            //    throw new CalcoloRischioDifesaDataReadTimeoutException("Timeout fase acquisizione modelli difesa raggiunto.", ex);
            //}            
        }
    }
}
