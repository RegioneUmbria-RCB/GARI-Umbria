using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.OutData.Pratiche
{
    /// <summary>
    /// Output preview visibilità per una configurazione di filtri pratiche.
    /// DS06-API – POST /v1/profilo-utente/pratiche/{idUtente}/preview.
    /// TODO: DS04-BL – calcolo visibilità centralizzata non ancora implementato.
    /// </summary>
    public class PreviewProfiloUtenteFiltriPratiche_OUT
    {
        public bool Success { get; set; }
        public string IdUtente { get; set; } = string.Empty;
        public string OperatoreFiltri { get; set; } = string.Empty;
        public int NumPraticheFiltrate { get; set; }
        /// <summary>TODO: DS04-BL – sempre 0 finché visibilità centralizzata non è implementata.</summary>
        public int NumAziendeVisibili { get; set; }
        /// <summary>TODO: DS04-BL – sempre 0 finché visibilità centralizzata non è implementata.</summary>
        public int NumAziendeTotali { get; set; }
        /// <summary>TODO: DS04-BL – sempre false finché visibilità centralizzata non è implementata.</summary>
        public bool VisibilitaTotale { get; set; }
        public List<MessaggioAvviso_OUT> MessaggiAvviso { get; set; } = new List<MessaggioAvviso_OUT>();
        public DateTime TimestampPreview { get; set; }
    }
}
