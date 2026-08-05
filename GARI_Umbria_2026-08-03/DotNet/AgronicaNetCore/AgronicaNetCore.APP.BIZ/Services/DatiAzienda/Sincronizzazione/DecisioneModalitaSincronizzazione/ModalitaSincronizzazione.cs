namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Well-known synchronisation mode codes returned by DS03.
/// Ref: DS03-BL – Output: modalita_sincronizzazione.
/// </summary>
public static class ModalitaSincronizzazione
{
    public const string FullSyncCompleta = "FULL_SYNC_COMPLETA";
    public const string FullSyncParziale = "FULL_SYNC_PARZIALE";
    public const string IncrementalSync = "INCREMENTAL_SYNC";
}