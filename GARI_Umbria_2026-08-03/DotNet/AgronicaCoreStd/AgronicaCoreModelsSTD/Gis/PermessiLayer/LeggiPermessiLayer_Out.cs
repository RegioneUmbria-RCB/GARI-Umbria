using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis.PermessiLayer
{
    /// <summary>
    /// Lista permessi per Layer singolo
    /// </summary>
    public class LeggiPermessiLayer_Out
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int LayerCod { get; set; }

        /// <summary>
        /// Descrizione Layer (letta da anagrafica)
        /// </summary>
        /// <example> 11 </example>
        public string LayerDescr { get; set; }

        /// <summary>
        /// Lista permessi specifici per utente
        /// </summary>
        public List<PermessiXUtente> utenti_permessi { get; set; }

        /// <summary>
        /// Lista permessi specifici per gruppo utente
        /// </summary>
        public List<PermessiXGruppiUtente> gruppiutente_permessi { get; set; }

        public LeggiPermessiLayer_Out()
        {
            LayerCod = 0;
            LayerDescr = "";
            utenti_permessi = new List<PermessiXUtente>();
            gruppiutente_permessi = new List<PermessiXGruppiUtente>();
        }
    }

    public class LeggiPermessiLayerUtenti_Out
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int LayerCod { get; set; }

        /// <summary>
        /// Descrizione Layer (letta da anagrafica)
        /// </summary>
        /// <example> 11 </example>
        public string LayerDescr { get; set; }

        /// <summary>
        /// Lista permessi specifici per utente
        /// </summary>
        public List<PermessiXUtente> utenti_permessi { get; set; }

        public LeggiPermessiLayerUtenti_Out()
        {
            LayerCod = 0;
            LayerDescr = "";
            utenti_permessi = new List<PermessiXUtente>();
        }
    }

    public class LeggiPermessiLayerGruppiUtente_Out
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int LayerCod { get; set; }

        /// <summary>
        /// Descrizione Layer (letta da anagrafica)
        /// </summary>
        /// <example> 11 </example>
        public string LayerDescr { get; set; }

        /// <summary>
        /// Lista permessi specifici per gruppo utente
        /// </summary>
        public List<PermessiXGruppiUtente> gruppiutente_permessi { get; set; }

        public LeggiPermessiLayerGruppiUtente_Out()
        {
            LayerCod = 0;
            LayerDescr = "";
            gruppiutente_permessi = new List<PermessiXGruppiUtente>();
        }
    }

}
