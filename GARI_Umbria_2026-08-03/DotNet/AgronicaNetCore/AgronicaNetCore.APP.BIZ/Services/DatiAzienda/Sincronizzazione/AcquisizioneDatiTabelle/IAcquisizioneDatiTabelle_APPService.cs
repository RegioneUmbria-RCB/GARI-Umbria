namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AcquisizioneDatiTabelle;

/// <summary>
/// Orchestrates the BIZ-service calls needed to acquire company data for all tables that
/// DS03 marked for synchronisation.
/// Ref: DS04-BL – Nome: AcquisizioneDatiTabelleCritiche.
/// </summary>
public interface IAcquisizioneDatiTabelle_APPService
{
    /// <summary>
    /// For each table in <paramref name="input"/> with
    /// <c>Sincronizzare = true</c>, calls the appropriate BIZ service and returns the
    /// aggregated result. Tables marked as not-to-sync receive their default empty value.
    /// Service invocation errors are caught, logged and replaced with an empty value so that
    /// the remaining tables are still processed.
    /// Ref: DS04-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    /// <exception cref="AgronicaNetCore.APP.BIZ.Exceptions.InvalidInputException">
    /// Thrown when <see cref="AcquisizioneDatiTabelle_Input.Piva"/> is null or empty.
    /// Ref: DS04-BL – Eccezioni: InvalidAziendaException.
    /// </exception>
    Task<AcquisizioneDatiTabelle_Result> AcquisisciDatiAsync(AcquisizioneDatiTabelle_Input input);
}
