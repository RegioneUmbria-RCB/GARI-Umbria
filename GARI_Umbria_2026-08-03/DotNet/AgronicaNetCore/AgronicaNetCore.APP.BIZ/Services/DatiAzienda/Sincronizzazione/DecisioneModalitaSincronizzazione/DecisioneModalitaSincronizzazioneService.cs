using AgronicaNetCore.APP.BIZ.Exceptions;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Implements the DS03 central decision logic that classifies the synchronisation as complete,
/// partial or incremental based on DS01 and DS02 outputs.
/// Ref: DS03-BL – Nome: DecisioneModalitaSincronizzazione.
/// </summary>
public sealed class DecisioneModalitaSincronizzazioneService : IDecisioneModalitaSincronizzazioneService
{
    private const string MotivoModificheRilevate = "modifiche_rilevate";
    private const string MotivoSincronizzazioneSempre = "sincronizzazione_sempre";
    private const string MotivoTimestampAppSuperiore = "timestamp_app_superiore";
    private const string MotivoPrimoAccesso = "prima_sincronizzazione";

    /// <inheritdoc/>
    public DecisioneModalitaSincronizzazioneResult Decidi(DecisioneModalitaSincronizzazioneInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ValidateInput(input);

        if (input.ForzaFullSyncParametri)
        {
            return BuildForcedFullSyncResult(input);
        }

        var tabellePerSincronizzazione = new Dictionary<string, DecisioneTabellaSincronizzazioneResult>(StringComparer.OrdinalIgnoreCase);
        foreach ((string nomeTabella, var decisioneTabella) in input.ModificheTabelle)
        {
            string motivo = decisioneTabella.Sincronizzare
                ? MapMotivoSincronizzazione(decisioneTabella.Motivo)
                : MotiviDecisioneModalitaSincronizzazione.NessunaModifica;

            tabellePerSincronizzazione[nomeTabella] = new DecisioneTabellaSincronizzazioneResult(
                decisioneTabella.Sincronizzare,
                motivo);
        }

        int tabelleSincronizzate = tabellePerSincronizzazione.Count(item => item.Value.Sincronizzare);
        string modalita = tabelleSincronizzate > 0
            ? ModalitaSincronizzazione.FullSyncParziale
            : ModalitaSincronizzazione.IncrementalSync;

        return BuildResult(modalita, tabellePerSincronizzazione, forzaFullSyncAttiva: false);
    }

    private static DecisioneModalitaSincronizzazioneResult BuildForcedFullSyncResult(
        DecisioneModalitaSincronizzazioneInput input)
    {
        string motivo = input.ParametriCoerenti
            ? MotiviDecisioneModalitaSincronizzazione.PrimoAccesso
            : MotiviDecisioneModalitaSincronizzazione.DivergenzaParametri;

        var tabellePerSincronizzazione = new Dictionary<string, DecisioneTabellaSincronizzazioneResult>(StringComparer.OrdinalIgnoreCase);
        foreach (string nomeTabella in input.ModificheTabelle.Keys)
        {
            tabellePerSincronizzazione[nomeTabella] = new DecisioneTabellaSincronizzazioneResult(true, motivo);
        }

        return BuildResult(
            ModalitaSincronizzazione.FullSyncCompleta,
            tabellePerSincronizzazione,
            forzaFullSyncAttiva: true);
    }

    private static DecisioneModalitaSincronizzazioneResult BuildResult(
        string modalita,
        IReadOnlyDictionary<string, DecisioneTabellaSincronizzazioneResult> tabellePerSincronizzazione,
        bool forzaFullSyncAttiva)
    {
        int totaleTabelle = tabellePerSincronizzazione.Count;
        int tabelleSincronizzate = tabellePerSincronizzazione.Count(item => item.Value.Sincronizzare);
        int tabelleOmesse = totaleTabelle - tabelleSincronizzate;

        var riassunto = new RiassuntoDecisioneSincronizzazioneResult(
            totaleTabelle,
            tabelleSincronizzate,
            tabelleOmesse,
            forzaFullSyncAttiva);

        return new DecisioneModalitaSincronizzazioneResult(modalita, tabellePerSincronizzazione, riassunto);
    }

    private static void ValidateInput(DecisioneModalitaSincronizzazioneInput input)
    {
        if (!input.ForzaFullSyncParametri && !input.ParametriCoerenti)
        {
            throw new InvalidInputException(
                "Input DS03 incoerente: parametri divergenti richiedono forza_full_sync_parametri = true. Ref: DS03-BL.");
        }

        if (input.ModificheTabelle is null || input.ModificheTabelle.Count == 0)
        {
            throw new InvalidInputException(
                "Input DS03 incompleto: modifiche_tabelle deve contenere almeno una tabella. Ref: DS03-BL.");
        }

        foreach ((string nomeTabella, var decisioneTabella) in input.ModificheTabelle)
        {
            if (string.IsNullOrWhiteSpace(nomeTabella))
            {
                throw new InvalidInputException(
                    "Input DS03 incompleto: ogni tabella deve avere un nome valorizzato. Ref: DS03-BL.");
            }

            if (decisioneTabella is null)
            {
                throw new InvalidInputException(
                    $"Input DS03 incompleto: la tabella '{nomeTabella}' non contiene il risultato DS02. Ref: DS03-BL.");
            }

            if (string.IsNullOrWhiteSpace(decisioneTabella.Motivo))
            {
                throw new InvalidInputException(
                    $"Input DS03 incompleto: la tabella '{nomeTabella}' non contiene il motivo DS02. Ref: DS03-BL.");
            }
        }
    }

    private static string MapMotivoSincronizzazione(string motivoDs02)
    {
        string normalizedMotivo = motivoDs02.Trim().ToLowerInvariant();
        return normalizedMotivo switch
        {
            MotivoModificheRilevate => MotiviDecisioneModalitaSincronizzazione.ModificheTimestamp,
            MotivoSincronizzazioneSempre => MotiviDecisioneModalitaSincronizzazione.NoLogDisponibile,
            MotivoTimestampAppSuperiore => MotiviDecisioneModalitaSincronizzazione.NessunaModifica,
            MotivoPrimoAccesso => MotiviDecisioneModalitaSincronizzazione.PrimoAccesso,
            _ => throw new DecisionLogicException(
                $"Motivo DS02 non supportato per la decisione DS03: '{motivoDs02}'. Ref: DS03-BL.")
        };
    }
}