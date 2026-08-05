namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Minimal DAL contract required by <see cref="IRaccoltaJsonMemoriaService.CommitAsync"/> to persist
/// the collected JSON entries atomically in <c>app_preparazione_daticomuni_web2app</c>.
/// Implemented by the DS05 data-access layer.
/// Ref: DS04-BL – Persistenze Coinvolte.
/// </summary>
public interface IPreparazioneDatiComuniAggiornamentoDAL
{
    /// <summary>
    /// Persists all <paramref name="entries"/> in a single atomic database transaction.
    /// </summary>
    Task SalvaAsync(IReadOnlyCollection<PreparazioneDatiComuniEntry> entries);
}
