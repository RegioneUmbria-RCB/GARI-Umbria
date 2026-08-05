namespace AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Response
{
    /// <summary>Dettaglio di un singolo evento avverso restituito dall'Engine M2.</summary>
    public class EventoAvversoResponse
    {
        public string? data_inizio { get; set; }
        public int durata_giorni { get; set; }
        public double danno_popolazione_pct { get; set; }
        public string? fase_fenologica { get; set; }
        public object? dettaglio_modello { get; set; }
    }

    /// <summary>Statistiche aggregate di rischio per un tipo di avversità meteoclimatica.</summary>
    public class RischioAvversitaResponse
    {
        public double dannoPopolazionePctComb { get; set; }
        public int nEventi { get; set; }
        public double dannoPopolazionePctMax { get; set; }
        public double dannoPopolazionePctAvg { get; set; }
        public List<EventoAvversoResponse>? eventi_avversi { get; set; }
    }

    /// <summary>Singola anomalia (warning o error) restituita dall'Engine M2.</summary>
    public class AnomaliaResponse
    {
        public string? codice { get; set; }
        public string? descrizione { get; set; }
    }

    /// <summary>Anomalie (warning e errori non bloccanti) segnalate dall'Engine M2.</summary>
    public class AnomalieResponse
    {
        public List<AnomaliaResponse>? warning { get; set; }
        public List<AnomaliaResponse>? error { get; set; }
    }

    /// <summary>
    /// Risposta dell'endpoint <c>POST /v1/rischi/valutazione</c> dell'Engine M2.
    /// Riferimento spec: OpenAPI Engine Rischi Meteoclimatici M2.
    /// </summary>
    public class AssessRiskResponse
    {
        public RischioAvversitaResponse? rischioGelo { get; set; }
        public RischioAvversitaResponse? rischioSiccita { get; set; }
        public RischioAvversitaResponse? rischioAllagamento { get; set; }
        public AnomalieResponse? anomalie { get; set; }
    }

    /// <summary>Risposta di errore standard restituita dall'Engine M2.</summary>
    public class ErrorResponseM2
    {
        public string? error { get; set; }
        public string? message { get; set; }
    }

    /// <summary>Risposta di errore di validazione con dettaglio per campo, restituita dall'Engine M2.</summary>
    public class ValidationErrorResponseM2 : ErrorResponseM2
    {
        public Dictionary<string, string>? fieldErrors { get; set; }
    }
}
