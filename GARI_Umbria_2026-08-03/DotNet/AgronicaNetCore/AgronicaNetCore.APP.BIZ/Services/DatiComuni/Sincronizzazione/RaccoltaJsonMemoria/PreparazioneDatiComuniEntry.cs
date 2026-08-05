namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Represents a single table's JSON snapshot passed to the DAL during
/// <see cref="IRaccoltaJsonMemoriaService.CommitAsync"/>.
/// Ref: DS03-BL – Nota Tecnica: exists flag transferred to DS05-BL.
/// Ref: DS04-BL – Persistenze Coinvolte: app_preparazione_daticomuni_web2app (scrittura).
/// </summary>
public sealed class PreparazioneDatiComuniEntry
{
    /// <summary>Name of the common table this snapshot belongs to.</summary>
    public string NomeTabella { get; }

    /// <summary>Serialised JSON produced for this table in the current job cycle.</summary>
    public string JsonNuovo { get; }

    /// <summary>UTC timestamp at which the JSON was generated.</summary>
    public DateTime TimestampGenerazione { get; }

    /// <summary>
    /// <c>true</c> when a row already exists for this table (UPDATE required);
    /// <c>false</c> on first execution (INSERT required).
    /// Ref: DS03-BL – Output: exists; DS05-BL uses this to choose INSERT vs UPDATE.
    /// </summary>
    public bool Exists { get; }

    public PreparazioneDatiComuniEntry(string nomeTabella, string jsonNuovo, DateTime timestampGenerazione, bool exists)
    {
        NomeTabella = nomeTabella;
        JsonNuovo = jsonNuovo;
        TimestampGenerazione = timestampGenerazione;
        Exists = exists;
    }
}
