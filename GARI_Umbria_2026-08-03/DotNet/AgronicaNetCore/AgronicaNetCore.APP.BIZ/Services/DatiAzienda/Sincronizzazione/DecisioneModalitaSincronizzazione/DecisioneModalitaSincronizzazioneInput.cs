using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Consolidated inputs consumed by DS03 to select the synchronisation strategy.
/// Ref: DS03-BL – Input.
/// </summary>
public sealed class DecisioneModalitaSincronizzazioneInput
{
    /// <summary>
    /// True when DS01 found no divergence across stored parameters and current visibility.
    /// Ref: DS03-BL – Input: parametri_coerenti.
    /// </summary>
    public bool ParametriCoerenti { get; }

    /// <summary>
    /// True when DS01 requires a complete dataset because of divergence or first access.
    /// Ref: DS03-BL – Input: forza_full_sync_parametri.
    /// </summary>
    public bool ForzaFullSyncParametri { get; }

    /// <summary>
    /// Per-table timestamp/log evaluation results produced upstream by DS02.
    /// Ref: DS03-BL – Input: modifiche_tabelle.
    /// </summary>
    public IReadOnlyDictionary<string, VerificaTimestampTabellaResult> ModificheTabelle { get; }

    public DecisioneModalitaSincronizzazioneInput(
        bool parametriCoerenti,
        bool forzaFullSyncParametri,
        IReadOnlyDictionary<string, VerificaTimestampTabellaResult> modificheTabelle)
    {
        ParametriCoerenti = parametriCoerenti;
        ForzaFullSyncParametri = forzaFullSyncParametri;
        ModificheTabelle = modificheTabelle;
    }
}