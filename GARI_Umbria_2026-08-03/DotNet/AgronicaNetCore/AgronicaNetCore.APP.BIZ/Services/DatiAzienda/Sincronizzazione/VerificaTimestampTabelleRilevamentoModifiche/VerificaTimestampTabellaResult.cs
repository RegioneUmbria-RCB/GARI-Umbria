namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Per-table synchronisation decision produced by DS02.
/// Ref: DS02-BL – Output: modifiche_rilevate.[tabella].
/// </summary>
public sealed class VerificaTimestampTabellaResult
{
    /// <summary>
    /// True when the table must be synchronised, false when it can be omitted.
    /// Ref: DS02-BL – Output: sincronizzare.
    /// </summary>
    public bool Sincronizzare { get; }

    /// <summary>
    /// Reason code for the decision.
    /// Ref: DS02-BL – Output: motivo.
    /// </summary>
    public string Motivo { get; }

    public VerificaTimestampTabellaResult(bool sincronizzare, string motivo)
    {
        Sincronizzare = sincronizzare;
        Motivo = motivo;
    }
}