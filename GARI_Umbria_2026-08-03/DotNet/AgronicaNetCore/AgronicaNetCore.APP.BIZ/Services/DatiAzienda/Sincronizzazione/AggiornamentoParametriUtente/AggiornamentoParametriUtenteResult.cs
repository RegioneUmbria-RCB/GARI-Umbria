namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AggiornamentoParametriUtente;

/// <summary>
/// Result returned by <see cref="IAggiornamentoParametriUtenteService.AggiornaAsync"/>.
/// Ref: DS06-BL – Output.
/// </summary>
public sealed class AggiornamentoParametriUtenteResult
{
    /// <summary>
    /// The persistence operation that was applied.
    /// <c>"insert"</c> for a new record, <c>"update"</c> for an existing one.
    /// Ref: DS06-BL – Output: operazione.
    /// </summary>
    public string Operazione { get; }

    /// <summary>
    /// The UTC timestamp written to <c>timestamp_aggiornamento</c>.
    /// Ref: DS06-BL – Output: timestamp_aggiornamento.
    /// </summary>
    public DateTime TimestampAggiornamento { get; }

    /// <summary>
    /// Fixed value <c>"success"</c>.
    /// Ref: DS06-BL – Output: esito.
    /// </summary>
    public string Esito => "success";

    /// <summary>
    /// <c>true</c> when this was the first access (INSERT path), indicating the client must
    /// perform a full synchronisation.
    /// Ref: DS06-BL – Output: forza_full_sync.
    /// </summary>
    public bool ForzaFullSync { get; }

    public AggiornamentoParametriUtenteResult(string operazione, DateTime timestampAggiornamento, bool forzaFullSync)
    {
        Operazione = operazione;
        TimestampAggiornamento = timestampAggiornamento;
        ForzaFullSync = forzaFullSync;
    }
}
