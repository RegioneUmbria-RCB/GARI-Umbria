namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.SerializzazioneJSONPreparazionePayloadRisposta;

/// <summary>
/// Encapsulates the DS05-BL inputs required to build the final Dati Azienda response payload.
/// Ref: DS05-BL – Input.
/// </summary>
public sealed class SerializzazioneJSONPreparazionePayloadRispostaInput
{
    /// <summary>
    /// Dictionary containing the company-data tables to include in the payload.
    /// Tables not synchronized should be represented by an empty object ({}) or may be omitted,
    /// in which case DS05 emits {} placeholders using the fixed payload order.
    /// Ref: DS05-BL – Input: dati_tabelle.
    /// </summary>
    public IReadOnlyDictionary<string, object?> DatiTabelle { get; }

    /// <summary>
    /// ISO 8601 synchronization timestamp produced by DS01-BL.
    /// Ref: DS05-BL – Input: timestamp_sincronizzazione.
    /// </summary>
    public string TimestampSincronizzazione { get; }

    /// <summary>
    /// True when the client timestamp was ahead of the server timestamp and DS01 detected
    /// the clock-skew edge case.
    /// Ref: DS05-BL – Input: edge_case_clock_skew.
    /// </summary>
    public bool EdgeCaseClockSkew { get; }

    /// <summary>
    /// Creates a DS05-BL input instance.
    /// Ref: DS05-BL – Input.
    /// </summary>
    public SerializzazioneJSONPreparazionePayloadRispostaInput(
        IReadOnlyDictionary<string, object?> datiTabelle,
        string timestampSincronizzazione,
        bool edgeCaseClockSkew)
    {
        DatiTabelle = datiTabelle ?? throw new ArgumentNullException(nameof(datiTabelle));
        TimestampSincronizzazione = timestampSincronizzazione ?? throw new ArgumentNullException(nameof(timestampSincronizzazione));
        EdgeCaseClockSkew = edgeCaseClockSkew;
    }
}