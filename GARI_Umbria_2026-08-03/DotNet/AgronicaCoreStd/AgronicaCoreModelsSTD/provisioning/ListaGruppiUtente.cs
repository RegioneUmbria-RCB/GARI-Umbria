using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.provisioning
{
    /// <summary>
    /// Classe lista dati gruppo utente
    /// </summary>
    public class ListaGruppiUtente
    {
        /// <summary>
        /// Lista dati gruppo utente
        /// </summary>
        public List<DatiGruppoUtente> ListaDatiGruppoUtente { get; set; }
    }

    /// <summary>
    /// Dati gruppo utente
    /// </summary>
    public class DatiGruppoUtente

    {
        /// <summary>
        /// Codice gruppo utente
        /// </summary>
        public string Codice { get; set; }
        /// <summary>
        /// Descrizione gruppo utente
        /// </summary>
        public string Descrizione { get; set; }

        /// <summary>
        /// Identificativo del gruppo. Non è il codice univoco. Può essere modificato ed è visibile all'utente.
        /// </summary>
        public string Identificativo { get; set; } = "";
    }

}
