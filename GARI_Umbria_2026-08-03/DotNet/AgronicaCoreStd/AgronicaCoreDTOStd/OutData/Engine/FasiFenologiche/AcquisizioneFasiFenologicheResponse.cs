using AgronicaCoreModelsSTD.Engine;
using System;
using System.Collections.Generic;

namespace OutData.Engine.FasiFenologiche
{
    /// <summary>
    /// Risposta dell'orchestrazione dell'acquisizione fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: OrchestrationAcquisizioneFasiFenologiche - Output.
    /// </remarks>
    public class AcquisizioneFasiFenologicheResponse
    {
        public bool Esito { get; set; }

        /// <summary>UUID v4 generato all'inizio dell'orchestrazione, propagato a tutte le BL.</summary>
        public string CorrelationId { get; set; } = string.Empty;

        public IReadOnlyList<FaseFenologicaEngine> FasiFenologiche { get; set; } = new List<FaseFenologicaEngine>();
        public int DurataTotaleMs { get; set; }
        public DateTimeOffset TimestampAcquisizione { get; set; }

        public OrchestrationErrore Errore { get; set; }
    }

    public class OrchestrationErrore
    {
        public string Codice { get; set; } = string.Empty;
        public string Messaggio { get; set; } = string.Empty;
        public string DettaglioTecnico { get; set; } = string.Empty;
    }
}
