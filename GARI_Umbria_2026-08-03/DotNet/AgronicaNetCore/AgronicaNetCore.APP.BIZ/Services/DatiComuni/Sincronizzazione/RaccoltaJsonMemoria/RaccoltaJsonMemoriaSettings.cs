namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Configuration options for <see cref="RaccoltaJsonMemoriaService"/> memory thresholds.
/// Bind to appsettings via section <c>"RaccoltaJsonMemoria"</c>.
/// Ref: DS04-BL – Regole di Business: monitorare memoria allocata.
/// </summary>
public sealed class RaccoltaJsonMemoriaSettings
{
    public const string SectionName = "RaccoltaJsonMemoria";

    /// <summary>
    /// Byte threshold above which a warning is logged (default: 1 GB).
    /// Ref: DS04-BL – Regole di Business: warning log se supera 1 GB.
    /// </summary>
    public long WarnThresholdBytes { get; set; } = 1L * 1024 * 1024 * 1024;

    /// <summary>
    /// Byte threshold above which <see cref="Exceptions.MemoryExceededException"/> is thrown (default: 2 GB).
    /// Ref: DS04-BL – Eccezioni: MemoryExceededException, soglia configurabile.
    /// </summary>
    public long MaxThresholdBytes { get; set; } = 2L * 1024 * 1024 * 1024;
}
