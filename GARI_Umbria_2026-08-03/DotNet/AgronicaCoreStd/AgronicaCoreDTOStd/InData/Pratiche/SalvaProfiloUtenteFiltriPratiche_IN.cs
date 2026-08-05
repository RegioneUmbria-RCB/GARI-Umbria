using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Pratiche
{
    /// <summary>
    /// Input per la modifica del profilo utente con filtri pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Input).
    /// </summary>
    public class SalvaProfiloUtenteFiltriPratiche_IN
    {
        /// <summary>Username dell'utente target da configurare.</summary>
        public string IdUtente { get; set; } = string.Empty;

        /// <summary>Flag filtro attivo sul profilo. Se null viene auto-calcolato dalla presenza di pratiche selezionate.</summary>
        public bool FiltroPraticheAttivo { get; set; } = false;

        /// <summary>Operatore logico per i filtri: 'AND' o 'OR'. Default: 'OR'.</summary>
        public string OperatoreFiltri { get; set; } = "OR";

        /// <summary>Lista delle pratiche da associare al profilo utente.</summary>
        public List<PraticaFiltrata_IN> PraticheFiltrate { get; set; } = new List<PraticaFiltrata_IN>();

        /// <summary>Username dell'amministratore che effettua la modifica.</summary>
        public string OperatoreRichiesta { get; set; } = string.Empty;
    }

    /// <summary>
    /// Singola pratica selezionata per il filtro utente.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Input – praticheFiltrate).
    /// </summary>
    public class PraticaFiltrata_IN
    {
        /// <summary>Codice servizio (Servizi.Servizio_Cod).</summary>
        public int Servizio_Cod { get; set; }

        /// <summary>Se true, applica anche il filtro sulla validità temporale della pratica.</summary>
        public bool ConsideraValiditaTemporale { get; set; }
    }
}
