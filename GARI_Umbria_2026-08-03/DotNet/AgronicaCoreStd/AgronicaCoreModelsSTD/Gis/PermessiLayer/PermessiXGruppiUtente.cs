using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis.PermessiLayer
{
    /// <summary>
    /// Subset permessi specifici per gruppo utente
    /// </summary>
    public class PermessiXGruppiUtente
    {
        /// <summary>
        /// Codice gruppo utente
        /// </summary>
        /// <example> 11 </example>
        public int Gruppo { get; set; }

        /// <summary>
        /// Descrizione gruppo (letto da anagrafica)
        /// </summary>
        public string GruppoDescr { get; set; }

        /// <summary>
        /// Permesso di inserimento dati
        /// </summary>
        public int Flag_Inserimento { get; set; }

        /// <summary>
        /// Permesso di modifica dati
        /// </summary>
        public int Flag_Modifica { get; set; }

        /// <summary>
        /// Permesso di cancellazione dati
        /// </summary>
        public int Flag_Cancellazione { get; set; }

        /// <summary>
        /// Permesso di visualizzazione informazioni (popup)
        /// </summary>
        public int Flag_Informazioni { get; set; }

        /// <summary>
        /// Permesso per poteri ammnistrativi su Layer
        /// </summary>
        public int Flag_Amministrazione { get; set; }
        
        /// <summary>
        /// Permesso per rimozione Layer
        /// </summary>
        public int Flag_Rimozione { get; set; }
    }
}
