using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.AssemblyPayloadCo2;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.InvocazioneEngineCo2;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ValidazioneFinalePayloadCo2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.CalcoloSostenibilitaCO2
{
    /// <summary>
    /// Orchestratore del flusso completo di calcolo CO2 (DS03→DS06→DS07).
    /// Delega a <see cref="IAssemblyPayloadCo2FilieraAziendaService"/> (assembly),
    /// <see cref="IValidazioneFinalePayloadCo2Service"/> (validazione) e
    /// <see cref="IInvocazioneEngineCo2Service"/> (invocazione motore esterno),
    /// mantenendo il controller libero da logica di orchestrazione.
    /// </summary>
    public class CalcoloSostenibilitaCO2Service : BaseServiceSostenibilitaCO2Biz, ICalcoloSostenibilitaCO2Service
    {
        private readonly IAssemblyPayloadCo2FilieraAziendaService _assemblyService;
        private readonly IValidazioneFinalePayloadCo2Service _validazioneService;
        private readonly IInvocazioneEngineCo2Service _invocazioneService;

        public CalcoloSostenibilitaCO2Service(
            IAssemblyPayloadCo2FilieraAziendaService assemblyService,
            IValidazioneFinalePayloadCo2Service validazioneService,
            IInvocazioneEngineCo2Service invocazioneService,
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _assemblyService = assemblyService;
            _validazioneService = validazioneService;
            _invocazioneService = invocazioneService;
        }

        /// <inheritdoc/>
        public async Task<RispostaEngineCo2> CalcoloSostenibilitaCO2Async(
            AssemblyPayloadCo2Input input,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer
            )
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(objParametriServer);

            PayloadCo2Root assembledPayload = await _assemblyService.AssembleAsync(input, objParametriUtenti, objParametriServer);

            ValidazioneFinalePayloadCo2Output validazione = _validazioneService.Validate(assembledPayload);
            if (!validazione.ValidazioneEsito)
                throw new ValidazioneFinalePayloadCo2Exception(validazione);

            var imprese = _serviceProvider.GetRequiredService<IImpresa>();
            var cuaaFiliera = await imprese.CuaaFromPivaAsync(input.Perimetro.Filiera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{input.Perimetro.Filiera}'.");

            var invocazioneInput = new InvocazioneEngineCo2Input
            {
                PayloadValidato = validazione.PayloadValidato!,
                Filiera = cuaaFiliera,
                Anno = input.Perimetro.Anno,
                Modalita = input.Perimetro.Modalita,
                Username = objParametriUtenti.UtenteUsername ?? string.Empty
        };

            return await _invocazioneService.InvokeAsync(invocazioneInput, objParametriServer, objParametriSuperServer);
        }
    }
}
