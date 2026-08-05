namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Well-known reason codes returned by DS03 for each table decision.
/// Ref: DS03-BL – Output: tabelle_per_sincronizzazione.[tabella].motivo.
/// </summary>
public static class MotiviDecisioneModalitaSincronizzazione
{
    public const string DivergenzaParametri = "divergenza_parametri";
    public const string PrimoAccesso = "prima_sincronizzazione";
    public const string ModificheTimestamp = "modifiche_timestamp";
    public const string NoLogDisponibile = "no_log_disponibile";
    public const string NessunaModifica = "nessuna_modifica";
}