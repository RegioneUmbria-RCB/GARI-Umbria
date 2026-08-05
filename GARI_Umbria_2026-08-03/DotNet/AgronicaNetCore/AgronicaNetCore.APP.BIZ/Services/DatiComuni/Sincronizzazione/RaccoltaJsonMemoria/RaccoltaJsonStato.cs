namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Represents the lifecycle state of the in-memory JSON buffer during a single job cycle.
/// Ref: DS04-BL – Output: stato_raccolta.
/// </summary>
public enum RaccoltaJsonStato
{
    /// <summary>Entries are being accumulated; no commit has been requested.</summary>
    InProgress,

    /// <summary>All entries were committed successfully to the database.</summary>
    ReadyForCommit,

    /// <summary>The buffer was rolled back; no data has been persisted.</summary>
    RolledBack
}
