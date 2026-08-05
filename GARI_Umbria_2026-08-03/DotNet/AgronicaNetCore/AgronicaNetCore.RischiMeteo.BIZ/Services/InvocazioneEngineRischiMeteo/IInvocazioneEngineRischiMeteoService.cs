using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Request;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Response;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.InvocazioneEngineRischiMeteo
{
    /// <summary>
    /// Input per l'invocazione del motore M2 per un singolo Esercizio.
    /// </summary>
    public class InvocazioneEngineRischiMeteoInput
    {
        public int Id { get; set; }
        /// <summary>Esercizio colturale di riferimento.</summary>
        public EsercizioRischiMeteoInput Esercizio { get; init; } = new();

        /// <summary>Payload strutturato pronto per l'invio all'Engine M2.</summary>
        public AssessRiskRequest Payload { get; init; } = new();

        /// <summary>Username dell'utente che ha avviato il calcolo.</summary>
        public string Username { get; init; } = string.Empty;

        /// <summary>Dati di identificazione della Filiera.</summary>
        public string IdFiliera { get; init; } = string.Empty;

        /// <summary>Dati di identificazione del Prodotto FMP.</summary>
        public string? CodProdottoFmp { get; init; }

        /// <summary>Nome prodotto FMP.</summary>
        public string? NomeProdotto { get; init; }

        /// <summary>Avvisi non bloccanti generati durante la costruzione del payload (pass-through al risultato).</summary>
        public IReadOnlyList<string> Avvisi { get; init; } = Array.Empty<string>();
    }

    /// <summary>Risultato dell'invocazione Engine M2 per un singolo Esercizio.</summary>
    public class InvocazioneEngineRischiMeteoRisultato
    {
        public EsercizioRischiMeteoInput Esercizio { get; init; } = new();
        public AssessRiskResponse? Risposta { get; init; }
        public IReadOnlyList<string> Avvisi { get; init; } = Array.Empty<string>();
        public bool Successo { get; init; }
        public string? Errore { get; init; }
    }

    /// <summary>
    /// Contratto per la business logic di invocazione dell'Engine Rischi Meteoclimatici (M2).
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Flusso invocazione Engine.
    /// </summary>
    public interface IInvocazioneEngineRischiMeteoService
    {
        /// <summary>
        /// Invia i payload all'Engine M2 per una lista di Esercizi, gestendo il parallelismo
        /// in batch da 5, persiste i dati di lookup e restituisce i risultati per Esercizio.
        /// </summary>
        Task<IList<InvocazioneEngineRischiMeteoRisultato>> InvokeAsync(IList<InvocazioneEngineRischiMeteoInput> inputs, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
