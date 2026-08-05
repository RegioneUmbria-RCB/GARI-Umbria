using System.Diagnostics;
using System.Security.Claims;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models.Api;
using AgronicaNetCore.SmartTractor.BIZ.Services.GestoreInvio;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiPermessi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgronicaNetCore.SmartTractor.API.Controllers;

/// <summary>
/// Thin HTTP controller for the Smart Tractor activity-send flow.
/// Delegates all orchestration to <see cref="IGestoreInvioSmartTractorService"/>.
///
/// Responsibilities:
/// 1. Extract authenticated-user identity from JWT.
/// 2. Validate the request model via ASP.NET Core model binding.
/// 3. Verify the user holds the <c>SmartTractor_FullAccess</c> permission.
/// 4. Build <see cref="AgronicaCoreParametriServer"/> from the current HTTP context.
/// 5. Translate BIZ exceptions to appropriate HTTP status codes.
///
/// No business logic lives here — it all resides in <see cref="IGestoreInvioSmartTractorService"/>
/// and its collaborators (DS03-BLb, DS03-BL, DS04-BL).
///
/// Referenced in Design Specification: DS04-BLb - Gestione invio a Smart Tractor, API interna.
/// </summary>
[ApiController]
[Route("api/smarttractor")]
[Authorize]
[Produces("application/json")]
public class SmartTractorController : ControllerBase
{
    private readonly IGestoreInvioSmartTractorService _gestoreInvioService;
    private readonly IUtentiPermessiService _utentiPermessiService;
    private readonly ILogger<SmartTractorController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="SmartTractorController"/>.
    /// </summary>
    public SmartTractorController(
        IGestoreInvioSmartTractorService gestoreInvioService,
        IUtentiPermessiService utentiPermessiService,
        ILogger<SmartTractorController> logger)
    {
        _gestoreInvioService = gestoreInvioService
            ?? throw new ArgumentNullException(nameof(gestoreInvioService));
        _utentiPermessiService = utentiPermessiService
            ?? throw new ArgumentNullException(nameof(utentiPermessiService));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends an Agronica activity to the Smart Tractor provider API.
    ///
    /// Full orchestration: validation → permission check → payload composition →
    /// outbound HTTP send → atomic DB persist.
    ///
    /// Referenced in Design Specification:
    /// DS04-BLb - Logica Orchestrazione, Endpoint POST /api/smarttractor/send-activity.
    /// </summary>
    /// <param name="request">Request body carrying activity ID, provider UUID, dates, and flags.</param>
    /// <param name="serverParams">Database connection parameters injected by the host application.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 with <see cref="SendActivitySuccessResponse"/> on success; HTTP 4xx/5xx otherwise.</returns>
    [HttpPost("send-activity")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(SendActivitySuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SendActivityErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(SendActivityErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SendActivityErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(SendActivityErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(SendActivityErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendActivityAsync(
        [FromBody] SendActivityRequest request,
        [FromServices] AgronicaCoreParametriServer serverParams,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var userId = ExtractUserId();

        _logger.LogInformation(
            "SmartTractor send-activity request received. ActivityId={ActivityId}, ProviderId={ProviderId}, UserId={UserId}",
            request.ActivityId, request.ProviderId, userId);

        // ===== FASE 1: MODEL VALIDATION (handled by ASP.NET Core model binding) =====
        if (!ModelState.IsValid)
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.ValidationFailed,
                "Dati non validi: campi obbligatori mancanti o formato errato.",
                errorDetails: ModelState));

        if (!IsValidSendMode(request.SendMode))
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.ValidationFailed,
                $"send_mode non valido: '{request.SendMode}'. Valori ammessi: 'quick', 'full'.",
                errorDetails: new { field = "send_mode", reason = "invalid enum value" }));

        // ===== FASE 2: PERMISSION CHECK =====
        try
        {
            var objParametriUtenti = BuildUtentiParametri(serverParams, userId);
            var hasPermission = await _utentiPermessiService.ReadAsync(
                objParametriUtenti,
                serverParams,
                idService: 5,
                idActivity: enum_Security_Attivita.SmartTractor_FullAccess,
                idOperation: 2);

            if (!hasPermission)
            {
                _logger.LogWarning(
                    "Permission denied for userId={UserId} on SmartTractor_FullAccess.", userId);
                return StatusCode(StatusCodes.Status403Forbidden, ErrorResponse(
                    SendActivityErrorCodes.PermissionDenied,
                    "Utente non autorizzato a inviare attività a Smart Tractor."));
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Permission check failed for userId={UserId}.", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse(
                SendActivityErrorCodes.InternalServerError,
                "Errore durante la verifica dei permessi."));
        }

        // ===== FASE 3: ORCHESTRATION =====
        var gestoreRequest = BuildGestoreInvioRequest(request, userId);
        try
        {
            var result = await _gestoreInvioService.InviaAttivitaAsync(
                gestoreRequest, serverParams, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation(
                "Activity sent successfully. AgronicaRequestId={RequestId}, TaskId={TaskId}, ResponseTimeMs={Ms}",
                result.AgronicaRequestId, result.ProviderResponseId, stopwatch.ElapsedMilliseconds);

            return Ok(new SendActivitySuccessResponse
            {
                AgronicaRequestId = result.AgronicaRequestId,
                SmartTractorTaskId = result.ProviderResponseId,
                Status             = result.Status,
                PayloadSizeBytes   = result.PayloadSizeBytes,
                Timestamp          = result.SentAt.ToString("o"),
                RequestMetadata    = new SendActivityRequestMetadata
                {
                    ActivityId    = request.ActivityId,
                    ProviderId    = request.ProviderId,
                    SendMode      = request.SendMode,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                }
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request cancelled for ActivityId={ActivityId}.", request.ActivityId);
            throw; // Let the host handle graceful shutdown.
        }
        catch (AttivitaValidationException ex)
        {
            _logger.LogWarning(ex, "Activity validation failed. ActivityId={ActivityId}.", request.ActivityId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.ValidationFailed,
                $"Validazione fallita: {ex.Message}"));
        }
        catch (RicettaNotFoundException ex)
        {
            _logger.LogWarning(ex, "Activity not found. ActivityId={ActivityId}.", request.ActivityId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.ValidationFailed,
                ex.Message,
                errorDetails: new { field = "activity_id", reason = "not found in database" }));
        }
        catch (MissingProviderMappingException ex)
        {
            _logger.LogWarning(ex, "Provider mapping missing. ActivityId={ActivityId}.", request.ActivityId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.PayloadCompositionFailed,
                ex.Message));
        }
        catch (MissingRequiredFieldException ex)
        {
            _logger.LogWarning(ex, "Missing required field in DTO. ActivityId={ActivityId}.", request.ActivityId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.PayloadCompositionFailed,
                $"Composizione payload fallita: campo obbligatorio assente — {ex.FieldName}."));
        }
        catch (RasterUploadException ex)
        {
            _logger.LogError(ex, "Raster upload failed for map {MapId}.", ex.MapId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.PayloadCompositionFailed,
                $"Composizione payload fallita: upload mappa raster non riuscito — {ex.ErrorDetail}."));
        }
        catch (SchemaValidationException ex)
        {
            _logger.LogError(ex, "Payload schema validation failed. ActivityId={ActivityId}.", request.ActivityId);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.PayloadCompositionFailed,
                $"Composizione payload fallita: {ex.Message}"));
        }
        catch (PayloadSizeExceededException ex)
        {
            _logger.LogError(ex, "Payload size exceeded. ActivityId={ActivityId}, Bytes={Bytes}.",
                request.ActivityId, ex.ActualSizeBytes);
            return BadRequest(ErrorResponse(
                SendActivityErrorCodes.PayloadCompositionFailed,
                $"Composizione payload fallita: dimensione payload ecceduta ({ex.ActualSizeBytes} bytes)."));
        }
        catch (SmartTractorSendException ex) when (IsTimeoutError(ex))
        {
            _logger.LogError(ex, "Smart Tractor timeout. ActivityId={ActivityId}.", request.ActivityId);
            return StatusCode(StatusCodes.Status504GatewayTimeout, ErrorResponse(
                SendActivityErrorCodes.SmartTractorTimeout,
                "Smart Tractor non raggiungibile (timeout)."));
        }
        catch (SmartTractorSendException ex) when (IsUnavailableError(ex))
        {
            _logger.LogError(ex, "Smart Tractor unavailable. ActivityId={ActivityId}.", request.ActivityId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ErrorResponse(
                SendActivityErrorCodes.SmartTractorUnavailable,
                "Smart Tractor non disponibile."));
        }
        catch (SmartTractorSendException ex)
        {
            _logger.LogError(ex, "Smart Tractor send failed. ActivityId={ActivityId}.", request.ActivityId);
            return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse(
                SendActivityErrorCodes.InternalServerError,
                $"Errore invio Smart Tractor: {ex.ErrorDetail}."));
        }
        catch (DatabaseTransactionException ex)
        {
            _logger.LogError(ex, "Database transaction failed. ActivityId={ActivityId}.", request.ActivityId);
            return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse(
                SendActivityErrorCodes.DatabaseTransactionFailed,
                "Errore salvataggio tracciamento richiesta.",
                errorDetails: new { reason = "transaction rollback after successful ST invio" }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error. ActivityId={ActivityId}.", request.ActivityId);
            return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse(
                SendActivityErrorCodes.InternalServerError,
                "Errore interno del server."));
        }
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the authenticated user ID from the JWT sub claim, falling back to
    /// <c>Identity.Name</c> when sub is absent.
    /// Referenced in Design Specification: DS04-BLb - Fase 1 (Estrae user_id da token).
    /// </summary>
    private string ExtractUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? User.Identity?.Name
        ?? string.Empty;

    /// <summary>
    /// Builds the <see cref="AgronicaCoreParametriUtenti"/> required by
    /// <see cref="IUtentiPermessiService"/> from the current user identity.
    /// </summary>
    private AgronicaCoreParametriUtenti BuildUtentiParametri(
        AgronicaCoreParametriServer serverParams, string userId)
    {
        return new AgronicaCoreParametriUtenti(
            serverParams.PivaSuperUser,
            serverParams.UsernameOperazione,
            utenteUsername:        userId,
            utenteCodFiscale:      string.Empty,
            superUserUsername:     serverParams.SuperUserUsername,
            finestraTemporaleInizio: serverParams.FinestraTemporaleInizio,
            finestraTemporaleFine:   serverParams.FinestraTemporaleFine,
            flagVisibilita:    AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
            flagCancellazioneLogica: AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica,
            stringaConnessione:  serverParams.StringaConnessione,
            logDirectory:        serverParams.LogDirectory,
            logFileName:         serverParams.LogFileName,
            logDescrizioneUtente: userId);
    }

    /// <summary>
    /// Maps a <see cref="SendActivityRequest"/> to a <see cref="GestoreInvioRequest"/>.
    /// </summary>
    private static GestoreInvioRequest BuildGestoreInvioRequest(
        SendActivityRequest request, string userId)
    {
        return new GestoreInvioRequest
        {
            AttivitaId            = request.ActivityId,
            RicettaOperazioneId   = request.RicettaOperazioneId,
            ProviderId            = request.ProviderId,
            CurrentUserId         = userId,
            PlannedStartDate      = request.PlannedStartDate,
            PlannedEndDate        = request.PlannedEndDate,
            IncludeProducts       = request.IncludeProducts,
            IncludePrescriptionMaps = request.IncludePrescriptionMap,
            SelectedPlantsIds     = request.SelectedPlantsIds
        };
    }

    /// <summary>
    /// Validates that <paramref name="sendMode"/> is one of the accepted enum values.
    /// Referenced in Design Specification: DS04-BLb - Validazioni Input (send_mode).
    /// </summary>
    private static bool IsValidSendMode(string sendMode) =>
        sendMode is Models.Api.SendMode.Quick or Models.Api.SendMode.Full;

    /// <summary>Builds a typed error response body.</summary>
    private static SendActivityErrorResponse ErrorResponse(
        string errorCode,
        string message,
        object? errorDetails = null) =>
        new()
        {
            ErrorCode    = errorCode,
            Message      = message,
            Timestamp    = DateTime.UtcNow.ToString("o"),
            ErrorDetails = errorDetails
        };

    // ─── SmartTractorSendException heuristics ───────────────────────────────

    /// <summary>
    /// Returns <c>true</c> when the send exception was caused by a timeout condition.
    /// Referenced in Design Specification: DS04-BLb - HTTP 504 Gateway Timeout.
    /// </summary>
    private static bool IsTimeoutError(SmartTractorSendException ex) =>
        ex.ErrorDetail.Contains("timeout", StringComparison.OrdinalIgnoreCase)
        || ex.InnerException is TimeoutException
        || ex.InnerException is TaskCanceledException;

    /// <summary>
    /// Returns <c>true</c> when the send exception was caused by a connection-refused condition.
    /// Referenced in Design Specification: DS04-BLb - HTTP 503 Service Unavailable.
    /// </summary>
    private static bool IsUnavailableError(SmartTractorSendException ex) =>
        ex.ErrorDetail.Contains("unavailable", StringComparison.OrdinalIgnoreCase)
        || ex.ErrorDetail.Contains("connection refused", StringComparison.OrdinalIgnoreCase)
        || ex.InnerException is HttpRequestException;
}
