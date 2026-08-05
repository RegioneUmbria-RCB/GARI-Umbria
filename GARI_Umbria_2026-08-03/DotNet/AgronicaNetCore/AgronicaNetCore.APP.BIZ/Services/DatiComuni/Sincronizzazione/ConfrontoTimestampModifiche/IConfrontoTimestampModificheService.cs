using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.ConfrontoTimestampModifiche;

/// <summary>
/// Compares the client's last-successful-sync timestamp against
/// <c>MAX(timestamp_aggiornamento)</c> in <c>app_preparazione_daticomuni_web2app</c>
/// to determine whether new data must be sent.
/// Ref: DS08-BL – Nome: ConfrontoTimestampModifiche.
/// </summary>
public interface IConfrontoTimestampModificheService
{
    /// <summary>
    /// Validates <paramref name="timestampClient"/> and compares it against the server's
    /// maximum update timestamp for <paramref name="tabelleRichieste"/>.
    /// Returns a <see cref="ConfrontoTimestampModificheResult"/> indicating whether changes
    /// exist and the authoritative server timestamp.
    /// Ref: DS08-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="timestampClient">
    /// UTC <see cref="DateTime"/> of the client's last successful synchronisation.
    /// Must not be more than one minute ahead of server UTC time; otherwise
    /// <see cref="InvalidTimestampException"/> is thrown.
    /// </param>
    /// <param name="tabelleRichieste">
    /// Names of the common tables whose timestamps must be inspected (≤ 36).
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    /// <exception cref="InvalidTimestampException">
    /// Thrown when <paramref name="timestampClient"/> is more than one minute in the future
    /// relative to server UTC time (possible clock skew or manipulation).
    /// Ref: DS08-BL – Regole di Business: Validare timestamp_client.
    /// </exception>
    /// <exception cref="Exceptions.ComparisonTimeoutException">
    /// Thrown when the combined DB query and comparison time exceeds 50 ms.
    /// Ref: DS08-BL – Eccezioni: ComparisonTimeoutException.
    /// </exception>
    Task<ConfrontoTimestampModificheResult> ConfrontaAsync(
        DateTime timestampClient,
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer);
}
