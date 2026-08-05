using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.OutData.Pratiche
{
    /// <summary>
    /// Output dell'operazione di salvataggio/lettura del profilo utente con filtri pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Output).
    /// </summary>
    public class SalvaProfiloUtenteFiltriPratiche_OUT
    {
        /// <summary>Indica se l'operazione è andata a buon fine.</summary>
        public bool Success { get; set; }

        /// <summary>Username dell'utente target configurato.</summary>
        public string IdUtente { get; set; } = string.Empty;

        /// <summary>Valore finale del flag FiltroPraticheAttivo dopo il salvataggio.</summary>
        public bool FiltroPraticheAttivo { get; set; }

        /// <summary>Valore finale dell'operatore logico (AND/OR) dopo il salvataggio.</summary>
        public string OperatoreFiltri { get; set; } = string.Empty;

        /// <summary>Numero di pratiche associate dopo il salvataggio.</summary>
        public int NumPraticheFiltrate { get; set; }

        /// <summary>Messaggi di avviso non-bloccanti (es. AND con zero pratiche).</summary>
        public List<MessaggioAvviso_OUT> MessaggiAvviso { get; set; } = new List<MessaggioAvviso_OUT>();

        /// <summary>
        /// True se l'utente ha visibilità totale con la configurazione corrente.
        /// TODO: DS04-BL – calcolo visibilità centralizzata non ancora implementato, restituisce sempre false.
        /// </summary>
        public bool VisibilitaTotale { get; set; }

        /// <summary>Timestamp della modifica/lettura effettuata (UTC).</summary>
        public DateTime TimestampModifica { get; set; }

        /// <summary>Identificativo audit log generato per questa modifica.</summary>
        public string AuditId { get; set; } = string.Empty;

        /// <summary>
        /// Lista delle pratiche associate al profilo utente, arricchite con i dati del catalogo.
        /// Popolata nella GET; vuota nella PATCH (non necessario nel response di modifica).
        /// </summary>
        public List<PraticaFiltrataDettaglio_OUT> PraticheFiltrate { get; set; } = new List<PraticaFiltrataDettaglio_OUT>();
    }
}
