namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggiornamentoParametriUtente;

/// <summary>
/// Result returned by <see cref="IAggiornamentoParametriUtenteService.AggiornаAsync"/>.
/// Ref: DS10-BL – Output.
/// </summary>
public sealed class AggiornamentoParametriUtenteResult
{
    /// <summary>
    /// The persistence operation that was applied.
    /// <c>"insert"</c> for a new record, <c>"update"</c> for an existing one.
    /// Ref: DS10-BL – Output: operazione.
    /// </summary>
    public string Operazione { get; }

    /// <summary>
    /// The UTC timestamp that was written to <c>timestamp_aggiornamento</c>.
    /// Ref: DS10-BL – Output: timestamp_aggiornamento.
    /// </summary>
    public DateTime TimestampAggiornamento { get; }

    /// <summary>
    /// Fixed value <c>"success"</c>.
    /// Ref: DS10-BL – Output: esito.
    /// </summary>
    public string Esito => "success";

    public AggiornamentoParametriUtenteResult(string operazione, DateTime timestampAggiornamento)
    {
        Operazione = operazione;
        TimestampAggiornamento = timestampAggiornamento;
    }
}
