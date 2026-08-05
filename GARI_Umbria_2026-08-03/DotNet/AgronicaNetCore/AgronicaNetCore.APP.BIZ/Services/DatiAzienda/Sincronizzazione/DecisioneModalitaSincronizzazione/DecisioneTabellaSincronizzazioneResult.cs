namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Final per-table DS03 decision indicating whether the table payload must be included.
/// Ref: DS03-BL – Output: tabelle_per_sincronizzazione.[tabella].
/// </summary>
public sealed class DecisioneTabellaSincronizzazioneResult
{
    /// <summary>
    /// True when the table must be transmitted, false when the caller should send an empty JSON.
    /// Ref: DS03-BL – Output: sincronizzare.
    /// </summary>
    public bool Sincronizzare { get; }

    /// <summary>
    /// Reason code describing why the table is or is not synchronised.
    /// Ref: DS03-BL – Output: motivo.
    /// </summary>
    public string Motivo { get; }

    public DecisioneTabellaSincronizzazioneResult(bool sincronizzare, string motivo)
    {
        Sincronizzare = sincronizzare;
        Motivo = motivo;
    }
}