namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.ConfrontoJsonVariazioni;

/// <summary>
/// Output of <see cref="IConfrontoJsonVariazioni_Service.ConfrontaAsync"/>.
/// Ref: DS03-BL – Output.
/// </summary>
public sealed class ConfrontoJsonVariazioniResult
{
    /// <summary>
    /// <c>true</c> if the two JSON strings differ (update required);
    /// <c>false</c> if they are identical.
    /// Ref: DS03-BL – Output: ha_variazioni.
    /// </summary>
    public bool HaVariazioni { get; }

    /// <summary>
    /// <c>true</c> if a row already exists in <c>app_preparazione_daticomuni_web2app</c>;
    /// <c>false</c> if this is the first execution for the table (INSERT required).
    /// Ref: DS03-BL – Output: exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Milliseconds elapsed during the comparison.
    /// Ref: DS03-BL – Output: comparison_time_ms.
    /// </summary>
    public long ComparisonTimeMs { get; }

    public ConfrontoJsonVariazioniResult(bool haVariazioni, bool exists, long comparisonTimeMs)
    {
        HaVariazioni = haVariazioni;
        Exists = exists;
        ComparisonTimeMs = comparisonTimeMs;
    }
}
