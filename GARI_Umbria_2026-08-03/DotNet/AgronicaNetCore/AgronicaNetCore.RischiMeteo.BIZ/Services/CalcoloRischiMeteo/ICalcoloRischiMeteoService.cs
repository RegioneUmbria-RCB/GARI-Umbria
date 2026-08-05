using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Response;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.CalcoloRischiMeteo
{
    /// <summary>Input per il calcolo dei rischi meteoclimatici per una Filiera.</summary>
    public class CalcoloRischiMeteoInput
    {
        /// <summary>Identificativo della Filiera.</summary>
        public string PivaFiliera { get; init; } = string.Empty;

        /// <summary>Esercizi selezionati per il calcolo.</summary>
        public IReadOnlyList<EsercizioRischiMeteoInput> Perimetro { get; init; } = Array.Empty<EsercizioRischiMeteoInput>();
    }

    /// <summary>Risultato per un singolo Esercizio elaborato.</summary>
    public class CalcoloRischiMeteoEsercizioRisultato
    {
        public EsercizioRischiMeteoInput Esercizio { get; init; } = new();
        public AssessRiskResponse? Risposta { get; init; }
        public IReadOnlyList<string> Avvisi { get; init; } = Array.Empty<string>();
        public bool Successo { get; init; }
        public string? Errore { get; init; }
    }

    /// <summary>Risultato aggregato del calcolo rischi per tutti gli Esercizi della Filiera.</summary>
    public class CalcoloRischiMeteoOutput
    {
        public IReadOnlyList<CalcoloRischiMeteoEsercizioRisultato> Risultati { get; init; } =
            Array.Empty<CalcoloRischiMeteoEsercizioRisultato>();
        public IReadOnlyList<ErroreCostruzioneRischiMeteo> ErroriCostruzione { get; init; } =
            Array.Empty<ErroreCostruzioneRischiMeteo>();
        public int TotaleEsercizi { get; init; }
        public int TotaleSuccessi => Risultati.Count(r => r.Successo);
        public int TotaleErrori  => Risultati.Count(r => !r.Successo) + ErroriCostruzione.Count;
    }

    /// <summary>
    /// Contratto per l'orchestrazione del calcolo rischi meteoclimatici M2.
    /// Coordina la costruzione dei payload (<see cref="Services.AssemblyPayloadM2.IAssemblyPayloadM2Service"/>)
    /// e l'invocazione dell'Engine per ciascun Esercizio (<see cref="InvocazioneEngineRischiMeteo.IInvocazioneEngineRischiMeteoService"/>).
    /// </summary>
    public interface ICalcoloRischiMeteoService
    {
        /// <summary>
        /// Esegue il calcolo completo: assembla i payload M2 e chiama l'Engine M2 per ogni Esercizio.
        /// Gli errori per singolo Esercizio sono isolati.
        /// </summary>
        Task<CalcoloRischiMeteoOutput> CalcolaRischiMeteoAsync(
            CalcoloRischiMeteoInput input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
