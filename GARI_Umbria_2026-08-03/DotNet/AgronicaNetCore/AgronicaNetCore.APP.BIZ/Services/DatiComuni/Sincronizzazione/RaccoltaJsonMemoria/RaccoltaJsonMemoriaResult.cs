namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Status snapshot returned by <see cref="IRaccoltaJsonMemoriaService.AggiungiAsync"/>
/// and <see cref="IRaccoltaJsonMemoriaService.GetStato"/>.
/// Ref: DS04-BL – Output.
/// </summary>
public sealed class RaccoltaJsonMemoriaResult
{
    /// <summary>
    /// Number of JSON entries currently held in the buffer.
    /// Ref: DS04-BL – Output: json_raccolti_count.
    /// </summary>
    public int JsonRaccoltiCount { get; }

    /// <summary>
    /// Estimated memory used by the buffer in megabytes.
    /// Ref: DS04-BL – Output: memoria_allocata_mb.
    /// </summary>
    public double MemoriaAllocataMb { get; }

    /// <summary>
    /// Current lifecycle state of the buffer.
    /// Ref: DS04-BL – Output: stato_raccolta.
    /// </summary>
    public RaccoltaJsonStato StatoRaccolta { get; }

    public RaccoltaJsonMemoriaResult(int jsonRaccoltiCount, double memoriaAllocataMb, RaccoltaJsonStato statoRaccolta)
    {
        JsonRaccoltiCount = jsonRaccoltiCount;
        MemoriaAllocataMb = memoriaAllocataMb;
        StatoRaccolta = statoRaccolta;
    }
}
