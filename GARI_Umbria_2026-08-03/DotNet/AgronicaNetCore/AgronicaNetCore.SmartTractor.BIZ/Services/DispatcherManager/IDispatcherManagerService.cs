using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.GestoreInvio;

/// <summary>
/// Service interface for the DS04-BL orchestrator that drives the full activity
/// send flow to Smart Tractor:
/// <list type="number">
///   <item><description>PHASE 0 – Retrieve activity DTO via DS03-BLb (<see cref="Attivita.IPrescriptionService"/>).</description></item>
///   <item><description>PHASE 1 – Compose Smart Tractor payload via DS03-BL (<see cref="ComposizionePayload.IComposizionePayloadService"/>).</description></item>
///   <item><description>PHASE 2 – Persist outgoing request record with status PENDING.</description></item>
///   <item><description>PHASE 3 – Send payload to Smart Tractor provider API.</description></item>
///   <item><description>PHASE 4 – Update request status to SENT (success) or FAILED (error).</description></item>
/// </list>
/// This service coordinates all sub-services but does not contain business logic itself.
/// Referenced in Design Specification: DS04-BL - Gestore Invio e Transazione Smart Tractor
/// </summary>
public interface IDispatcherManagerService
{
    /// <summary>
    /// Executes the full activity send flow for a single Agronica activity.
    /// </summary>
    /// <param name="prescriptionData">
    /// Raw prescription data already read from the DB by <see cref="Prescription.IPrescriptionService.ReadPrescriptionDataAsync"/>.
    /// </param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns>
    /// A <see cref="GestoreInvioResponse"/> containing the Agronica request UUID,
    /// provider-assigned activity ID, final status, payload size, and send timestamp.
    /// </returns>
    /// <exception cref="Exceptions.AttivitaValidationException">When input parameters are invalid.</exception>
    /// <exception cref="Exceptions.RicettaNotFoundException">When the activity is not found in RICETTE.</exception>
    /// <exception cref="Exceptions.MissingProviderMappingException">When a plant or product has no provider mapping.</exception>
    /// <exception cref="Exceptions.MissingRequiredFieldException">When a mandatory payload field is absent in the DTO.</exception>
    /// <exception cref="Exceptions.RasterUploadException">When a raster map upload to the provider fails.</exception>
    /// <exception cref="Exceptions.SchemaValidationException">When the composed payload fails schema validation.</exception>
    /// <exception cref="Exceptions.PayloadSizeExceededException">When the payload exceeds 100 MB.</exception>
    /// <exception cref="Exceptions.DatabaseTransactionException">When the DB record insert or update fails.</exception>
    /// <exception cref="Exceptions.SmartTractorSendException">When the HTTP send to Smart Tractor fails.</exception>
    Task<IReadOnlyList<GestoreInvioResponse>> InviaAttivitaAsync(
        PrescriptionDataDTO prescriptionData,
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams,
        CancellationToken cancellationToken = default);
}
