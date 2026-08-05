using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Exceptions;
using AgronicaNetCore.RischiMeteo.BIZ.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Services.AssemblyPayloadRischiMeteo;
using AgronicaNetCore.RischiMeteo.BIZ.Services.InvocazioneEngineRischiMeteo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.CalcoloRischiMeteo
{
    /// <summary>
    /// Orchestratore del calcolo rischi meteoclimatici M2.
    /// Coordina: costruzione payload → invocazione Engine M2 per ogni Esercizio.
    /// I fallimenti per singolo Esercizio sono isolati (fail-safe).
    /// Riferimento spec: DS02-BL CostruttoPayloadM2.
    /// </summary>
    public class CalcoloRischiMeteoService : BaseServiceRischiMeteoBiz, ICalcoloRischiMeteoService
    {
        private readonly IAssemblyPayloadRischiMeteoService _assemblyPayloadRischiMeteoService;
        private readonly IInvocazioneEngineRischiMeteoService _invocazioneEngineRischiMeteoService;
        public CalcoloRischiMeteoService(
            IAssemblyPayloadRischiMeteoService assemblyPayloadRischiMeteoService,
            IInvocazioneEngineRischiMeteoService invocazioneEngineRischiMeteoService,
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _assemblyPayloadRischiMeteoService = assemblyPayloadRischiMeteoService;
            _invocazioneEngineRischiMeteoService = invocazioneEngineRischiMeteoService;
        }

        /// <inheritdoc/>
        public async Task<CalcoloRischiMeteoOutput> CalcolaRischiMeteoAsync(
            CalcoloRischiMeteoInput input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Perimetro.Count == 0)
                throw new ArgumentException("Specificare almeno un Esercizio.", nameof(input));

            LogInformation(
                "CalcoloRischiMeteo avviato – Filiera: {IdFiliera}, Esercizi: {Count}",
                objParametriServer, null, input.PivaFiliera, input.Perimetro.Count);

            // ────────────────────────────────────────────────────────────
            // 1. Costruisci tutti i payload M2
            // ────────────────────────────────────────────────────────────
            var assemblyInput = new CostruzionePayloadRischiMeteoInput
            {
                IdFiliera           = input.PivaFiliera,
                EserciziSelezionati = input.Perimetro
            };

            var assemblyOutput = await _assemblyPayloadRischiMeteoService.AssembleAsync(assemblyInput, objParametriServer);

            // ────────────────────────────────────────────────────────────
            // 2. Invoca Engine M2 per tutti gli Esercizi
            //    (gestione del parallelismo a batch interna al servizio)
            // ────────────────────────────────────────────────────────────

            var imprese = _serviceProvider.GetRequiredService<IImpresa>();
            var cuaaFiliera = await imprese.CuaaFromPivaAsync(input.PivaFiliera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{input.PivaFiliera}'.");

            var invocazioneInputs = assemblyOutput.Payloads
                .Select(p => new InvocazioneEngineRischiMeteoInput
                {
                    Esercizio = p.Esercizio,
                    Payload   = p.Payload,
                    IdFiliera = cuaaFiliera,
                    Username  = objParametriServer.UtenteUsername ?? string.Empty,
                    Avvisi    = p.Avvisi
                })
                .ToList();

            var invocazioneRisultati = await _invocazioneEngineRischiMeteoService.InvokeAsync(invocazioneInputs, objParametriServer, objParametriSuperServer);

            var risultati = invocazioneRisultati
                .Select(r => new CalcoloRischiMeteoEsercizioRisultato
                {
                    Esercizio = r.Esercizio,
                    Risposta  = r.Risposta,
                    Avvisi    = r.Avvisi,
                    Successo  = r.Successo,
                    Errore    = r.Errore
                })
                .ToList();

            LogInformation(
                "CalcoloRischiMeteo completato – Filiera: {IdFiliera}, Successi: {Ok}, Errori payload: {ErrPayload}, Errori engine: {ErrEngine}",
                objParametriServer, null, input.PivaFiliera, risultati.Count(r => r.Successo),
                assemblyOutput.TotaleErrori, risultati.Count(r => !r.Successo));

            return new CalcoloRischiMeteoOutput
            {
                Risultati         = risultati,
                ErroriCostruzione = assemblyOutput.Errori,
                TotaleEsercizi    = input.Perimetro.Count
            };
        }
    }
}
