using AgronicaCoreModelsSTD.Gis.PermessiLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Salvataggio permessi su singolo layer
    /// </summary>
    public class SalvaPermessiLayer_In
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int Layer_Cod { get; set; }

        /// <summary>
        /// Lista permessi specifici per utente
        /// </summary>
        public List<PermessiXUtente> utenti_permessi { get; set; }

        /// <summary>
        /// Lista permessi specifici per gruppo utente
        /// </summary>
        public List<PermessiXGruppiUtente> gruppiutente_permessi { get; set; }

        public SalvaPermessiLayer_In()
        {
            Layer_Cod = 0;
            utenti_permessi = new List<PermessiXUtente>();
            gruppiutente_permessi = new List<PermessiXGruppiUtente>();
        }
    }

    public class SalvaPermessiLayerUtenti_In
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int Layer_Cod { get; set; }

        /// <summary>
        /// Lista permessi specifici per utente
        /// </summary>
        public List<PermessiXUtente> utenti_permessi { get; set; }

        public SalvaPermessiLayerUtenti_In()
        {
            Layer_Cod = 0;
            utenti_permessi = new List<PermessiXUtente>();
        }
    }

    public class SalvaPermessiLayerGruppiUtente_In
    {
        /// <summary>
        /// Codice Layer
        /// </summary>
        /// <example> 11 </example>
        public int Layer_Cod { get; set; }

        /// <summary>
        /// Lista permessi specifici per gruppo utente
        /// </summary>
        public List<PermessiXGruppiUtente> gruppiutente_permessi { get; set; }

        public SalvaPermessiLayerGruppiUtente_In()
        {
            Layer_Cod = 0;
            gruppiutente_permessi = new List<PermessiXGruppiUtente>();
        }
    }



}
