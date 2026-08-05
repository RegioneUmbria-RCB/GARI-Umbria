namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.GestioneSemaforoJob;

/// <summary>
/// Result returned by <see cref="IGestioneSemaforoJobService.AggiornaSemaforoFineAsync"/>.
/// Ref: DS06-BL – Output (AggiornaSemaforoFine).
/// </summary>
public sealed class AggiornaSemaforoFineResult
{
    /// <summary>Total job duration in seconds.</summary>
    public int DurataSecondi { get; }

    /// <summary>'semaforo_aggiornato' when successfully updated.</summary>
    public string Esito { get; }

    public AggiornaSemaforoFineResult(int durataSecondi, string esito)
    {
        DurataSecondi = durataSecondi;
        Esito = esito;
    }
}
