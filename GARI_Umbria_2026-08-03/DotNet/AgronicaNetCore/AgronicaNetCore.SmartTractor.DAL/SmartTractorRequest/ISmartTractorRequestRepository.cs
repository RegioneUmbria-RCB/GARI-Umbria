using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.SmartTractorRequest;

/// <summary>
/// Interface for Data Access Layer operations on the <c>smart_tractor_request</c> table.
/// Persists outgoing request records after successful payload composition.
/// The actual INSERT is executed by FS003 (Gestore Invio) after composition completes.
/// Referenced in Design Specification: DS03-BL - Composizione Payload
/// (Tracciamento Persistente Post-Composizione)
/// </summary>
public interface ISmartTractorRequestRepository
{
    /// <summary>
    /// Inserts a new Smart Tractor request record with status <c>PENDING</c>.
    /// </summary>
    /// <param name="providerId">The string provider ID.</param>
    /// <param name="ricettaOperazioneCod">
    /// The Agronica ricetta ID (<c>Ricetta_Cod</c>) that originated the request.
    /// </param>
    /// <param name="serverParams">Database connection parameters.</param>
    /// <returns><c>true</c> if the insert succeeded, <c>false</c> otherwise.</returns>
    Task<bool> InsertRequestAsync(
        string providerId,
        int ricettaOperazioneCod,
        int macCod,
        string smartTractorPrescriptionId,
        AgronicaCoreParametriServer serverParams);

    /// <summary>
    /// Updates the status of an existing Smart Tractor request record after a send attempt.
    /// Sets <c>status</c>, <c>provider_response_id</c> (on success), <c>error_detail</c>
    /// (on failure), and <c>updated_at</c> / <c>updated_by</c>.
    /// Referenced in Design Specification: DS04-BL - Gestore Invio e Transazione Smart Tractor
    /// (Tracciamento Persistente Post-Invio)
    /// </summary>
    /// <param name="ricettaOperazioneCod">The integer id of the request to update.</param>
    /// <param name="status">New status value (e.g. "SENT", "FAILED").</param>
    /// <param name="providerResponseId">
    /// Activity ID returned by the provider on success; <c>null</c> on failure.
    /// </param>
    /// <param name="errorDetail">
    /// Error description when the send failed; <c>null</c> on success.
    /// </param>
    /// <param name="currentUserId">The authenticated user for audit columns.</param>
    /// <param name="serverParams">Database connection parameters.</param>
    /// <returns><c>true</c> if the update succeeded, <c>false</c> otherwise.</returns>
    Task<bool> UpdateRequestStatusAsync(
        int ricettaOperazioneCod,
        string status,
        long? providerResponseId,
        string? errorDetail,
        string currentUserId,
        AgronicaCoreParametriServer serverParams);
}
