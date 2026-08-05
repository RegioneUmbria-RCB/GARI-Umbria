namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.GestioneSemaforoJob;

/// <summary>
/// Result returned by <see cref="IGestioneSemaforoJobService.ImpostaSemaforoInizioAsync"/>.
/// Ref: DS06-BL – Output (ImpostaSemaforoInizio).
/// </summary>
public sealed class ImpostaSemaforoResult
{
    /// <summary>UTC timestamp registered in the semaphore record.</summary>
    public DateTime TimestampInizio { get; }

    /// <summary>'semaforo_impostato' when successfully inserted.</summary>
    public string Esito { get; }

    public ImpostaSemaforoResult(DateTime timestampInizio, string esito)
    {
        TimestampInizio = timestampInizio;
        Esito = esito;
    }
}
