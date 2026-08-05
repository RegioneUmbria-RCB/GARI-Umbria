using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.BIZ.Services.Engine;
using AgronicaNetCore.SmartTractor.DAL.SmartTractorRequest;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.SmartTractor.BIZ.Services.PayloadBuiler;
using static AgronicaCoreDTOStd.Compliance.AnalysisRequest;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.GestoreInvio;

/// <summary>
/// Orchestrates the full activity send flow to Smart Tractor by coordinating the
/// payload composition layer (DS03-BL), the persistence layer, and the outbound HTTP client.
/// Prescription data is already read and validated upstream by
/// <see cref="SmartTractorService.SendPrescriptionAsync"/>.
///
/// Flow (per DS04-BL specification):
/// PHASE 0 – Validate the incoming <see cref="PrescriptionDataDTO"/>.
/// PHASE 1 – Build <see cref="ComposizionePayloadRequest"/> and compose the Smart Tractor payload.
/// PHASE 2 – Persist outgoing request record with status PENDING.
/// PHASE 3 – Send payload to Smart Tractor provider API; update status to SENT or FAILED.
///
/// Referenced in Design Specification: DS04-BL - Gestore Invio e Transazione Smart Tractor
/// </summary>
public class DispatcherManagerService : BaseServiceSmartTractorBIZ, IDispatcherManagerService
{
    private readonly IPayloadBuilderService _payloadBuilderService;
    private readonly IEngineService _engineService;
    private readonly ISmartTractorRequestRepository _requestRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="DispatcherManagerService"/>.
    /// </summary>
    public DispatcherManagerService(
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer,
        IPayloadBuilderService payloadBuilderService,
        IEngineService engineService,
        ISmartTractorRequestRepository requestRepository)
        : base(provider, localizer)
    {
        _payloadBuilderService = payloadBuilderService
            ?? throw new ArgumentNullException(nameof(payloadBuilderService));
        _engineService = engineService
            ?? throw new ArgumentNullException(nameof(engineService));
        _requestRepository = requestRepository
            ?? throw new ArgumentNullException(nameof(requestRepository));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<GestoreInvioResponse>> InviaAttivitaAsync(
        PrescriptionDataDTO prescriptionData,
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams,
        CancellationToken cancellationToken = default)
    {
        var responses = new List<GestoreInvioResponse>();
        // ===== PHASE 0: VALIDATE INPUT =====
        // Basic consistency checks; richer validation was already done upstream in SmartTractorService.
        ValidateInput(prescriptionData);


        // ===== PHASE 1: COMPOSE PAYLOAD (DS03-BL) =====
        // Build a PrescriptionPayloadDTO from the raw data so the payload builder can operate on it.
        var composizioneOutputs = await _payloadBuilderService.BuildPayloadAsync(
            prescriptionData, serverParams, superServerParams, cancellationToken);

        foreach ( var output in composizioneOutputs )
        {
            // ===== PHASE 2: PERSIST REQUEST RECORD AS PENDING =====
            var inserted = await _requestRepository.InsertRequestAsync(
                output.Payload.providerCode,
                output.RicettaOperazioneCod,
                1,
                "",
                serverParams);

            if (!inserted)
                throw new DatabaseTransactionException(
                    $"Failed to insert smart_tractor_request record for uuid={output.RicettaOperazioneCod}.");

            // ===== PHASE 3: SEND TO SMART TRACTOR + UPDATE STATUS =====
            try
            {
                var sendResponse = await _engineService.SendPrescriptionAsync(
                    output.Payload, serverParams, superServerParams, cancellationToken);

                await UpdateStatusSafeAsync(
                    output.RicettaOperazioneCod,
                    sendResponse.status,
                    providerResponseId: sendResponse.ActivityId,
                    errorDetail: sendResponse.error,
                    serverParams.UsernameOperazione,
                    serverParams);

                responses.Add(new GestoreInvioResponse
                {
                    RicettaOperazioneCod = output.RicettaOperazioneCod,
                    ProviderResponseId = sendResponse.ActivityId,
                    MacCod = output.MacCod,
                    Success = sendResponse.status != SmartTractorRequestStatus.Failed,
                    PayloadSizeBytes = output.PayloadSizeBytes,
                    SentAt = DateTime.UtcNow,
                    Error = sendResponse.error
                });
            }
            catch (SmartTractorSendException ex)
            {
                await UpdateStatusSafeAsync(
                    output.RicettaOperazioneCod,
                    SmartTractorRequestStatus.Failed,
                    providerResponseId: null,
                    errorDetail: "SmartTractorSendException — see logs for detail.",
                    serverParams.UsernameOperazione,
                    serverParams);

                responses.Add(new GestoreInvioResponse
                {
                    RicettaOperazioneCod = output.RicettaOperazioneCod,
                    MacCod = output.MacCod,
                    ProviderResponseId = 0,
                    Success = false,
                    PayloadSizeBytes = output.PayloadSizeBytes,
                    SentAt = DateTime.UtcNow,
                    Error = ex.Message
                });

                LogError(
                    $"Non-fatal: failed to send prescription to Smart Tractor engine" +
                    $"for uuid={output.RicettaOperazioneCod}. Detail: {ex.Message}",
                    serverParams);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await UpdateStatusSafeAsync(
                    output.RicettaOperazioneCod,
                    SmartTractorRequestStatus.Failed,
                    providerResponseId: null,
                    errorDetail: ex.Message,
                    serverParams.UsernameOperazione,
                    serverParams);

                responses.Add(new GestoreInvioResponse
                {
                    RicettaOperazioneCod = output.RicettaOperazioneCod,
                    MacCod = output.MacCod,
                    ProviderResponseId = 0,
                    Success = false,
                    PayloadSizeBytes = output.PayloadSizeBytes,
                    SentAt = DateTime.UtcNow,
                    Error = ex.Message
                });

                LogError(
                    $"Non-fatal: failed to send prescription to Smart Tractor engine" +
                    $"for uuid={output.RicettaOperazioneCod}. Detail: {ex.Message}",
                    serverParams);
            }
        }

        return responses;
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Validates mandatory fields in the <see cref="PrescriptionDataDTO"/> handed off by
    /// <see cref="SmartTractorService.SendPrescriptionAsync"/>.
    /// Referenced in Design Specification: DS04-BL - PHASE 0.
    /// </summary>
    private static void ValidateInput(PrescriptionDataDTO prescriptionData)
    {
        if (prescriptionData is null)
            throw new AttivitaValidationException("prescriptionData must not be null.");

        if (prescriptionData.RicettaOperazioneCod == 0)
            throw new AttivitaValidationException("RicettaOperazioneCod must not be empty.");

        if (prescriptionData.LavCod <= 0)
            throw new AttivitaValidationException("LavCod must be > 0.");

        if (prescriptionData.MachineryIds.Count == 0)
            throw new AttivitaValidationException("At least one machinery must be present.");

        if (prescriptionData.Destinazioni.Count == 0)
            throw new AttivitaValidationException("At least one destination must be present.");
    }

    /// <summary>
    /// Attempts to update the request status in DB, swallowing any exception to avoid
    /// masking the original send error.
    /// Referenced in Design Specification: DS04-BL - PHASE 4.
    /// </summary>
    private async Task UpdateStatusSafeAsync(
        int ricettaOperazioneCod,
        string status,
        long? providerResponseId,
        string? errorDetail,
        string currentUserId,
        AgronicaCoreParametriServer serverParams)
    {
        try
        {
            await _requestRepository.UpdateRequestStatusAsync(
                ricettaOperazioneCod,
                status,
                providerResponseId,
                errorDetail,
                currentUserId,
                serverParams);
        }
        catch (Exception ex)
        {
            // Log the failure but do not rethrow: the primary exception (send failure or
            // success path) should reach the caller unaltered.
            LogWarning(
                $"Non-fatal: failed to update smart_tractor_request status to '{status}' " +
                $"for uuid={ricettaOperazioneCod}. Detail: {ex.Message}",
                serverParams);
        }
    }
}
