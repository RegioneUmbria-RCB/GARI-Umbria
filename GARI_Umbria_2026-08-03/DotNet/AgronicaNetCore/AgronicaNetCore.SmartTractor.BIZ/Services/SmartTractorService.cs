using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models.Api;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.BIZ.Services.GestoreInvio;
using AgronicaNetCore.SmartTractor.BIZ.Services.Permission;
using AgronicaNetCore.SmartTractor.BIZ.Services.Prescription;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiPermessi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AgronicaNetCore.SmartTractor.BIZ.Services
{
    public class SmartTractorService : BaseServiceSmartTractorBIZ, ISmartTractorService
    {
        private readonly IDispatcherManagerService _dispatcherManagerService;
        private readonly ISmartTractorPermissionService _permissionService;
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<SmartTractorService> _logger;

        public SmartTractorService(IServiceProvider provider, IStringLocalizer<Messages> localizer, ILogger<SmartTractorService> logger) : base(provider, localizer)
        {
            _dispatcherManagerService = _serviceProvider.GetRequiredService<IDispatcherManagerService>();
            _permissionService        = _serviceProvider.GetRequiredService<ISmartTractorPermissionService>();
            _prescriptionService      = _serviceProvider.GetRequiredService<IPrescriptionService>();
            _logger = logger;
        }

        /// <summary>
        /// Reads all raw prescription data for the given GUID key, validates that the
        /// prescription is consistent (machineries and destination plots must be present),
        /// then delegates the full send flow to the dispatcher.
        ///
        /// Referenced in Design Specification: DS04-BLb - Orchestrazione invio prescrizione
        /// </summary>
        public async Task<IReadOnlyList<GestoreInvioResponse>> SendPrescriptionAsync(
            int ricettaOperazioneCod,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            AgronicaCoreParametriUtenti userParams,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "SendPrescriptionAsync started. RicettaOperazioneCod={Guid}, UserId={UserId}",
                ricettaOperazioneCod, userParams.UsernameOperazione);

            // FASE 1 — Permission check
            var permission = await _permissionService.VerifySendPermissionAsync(userParams, serverParams, false);
            if (!permission.Authorized)
            {
                _logger.LogWarning(
                    "Permission denied for userId={UserId} on SmartTractor_FullAccess.",
                    userParams.UsernameOperazione);
                throw new AttivitaValidationException(
                    $"Utente '{userParams.UsernameOperazione}' non autorizzato a inviare prescrizioni a Smart Tractor.");
            }

            // FASE 2 — Read raw prescription data
            var prescriptionData = await _prescriptionService.ReadPrescriptionDataAsync(
                ricettaOperazioneCod, serverParams);

            // FASE 3 — Validate prescription completeness
            if (prescriptionData.MachineryIds.Count == 0)
                throw new DataConsistencyException(
                    $"Nessun macchinario trovato nella prescrizione {ricettaOperazioneCod}.");

            if (prescriptionData.Destinazioni.Count == 0)
                throw new DataConsistencyException(
                    $"Nessuna destinazione trovata nella prescrizione {ricettaOperazioneCod}.");

            // FASE 4 — Dispatch: payload composition, persistence, and outbound send
            return await _dispatcherManagerService.InviaAttivitaAsync(
                prescriptionData, serverParams, superServerParams, cancellationToken);
        }
    }
}
