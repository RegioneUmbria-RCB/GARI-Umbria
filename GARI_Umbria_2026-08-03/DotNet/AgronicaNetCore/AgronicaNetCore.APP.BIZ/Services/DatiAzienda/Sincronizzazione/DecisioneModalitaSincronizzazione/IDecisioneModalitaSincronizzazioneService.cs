using AgronicaNetCore.APP.BIZ.Exceptions;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Applies the DS03 prioritisation rules to determine the final synchronisation strategy.
/// Ref: DS03-BL – Nome: DecisioneModalitaSincronizzazione.
/// </summary>
public interface IDecisioneModalitaSincronizzazioneService
{
    /// <summary>
    /// Combines the DS01 full-sync flag with the DS02 per-table results and returns the final
    /// synchronisation mode.
    /// Ref: DS03-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <exception cref="InvalidInputException">
    /// Thrown when <paramref name="input"/> is incomplete or violates the DS03 invariants.
    /// </exception>
    /// <exception cref="DecisionLogicException">
    /// Thrown when the input cannot be mapped to an unambiguous DS03 decision.
    /// </exception>
    DecisioneModalitaSincronizzazioneResult Decidi(DecisioneModalitaSincronizzazioneInput input);
}