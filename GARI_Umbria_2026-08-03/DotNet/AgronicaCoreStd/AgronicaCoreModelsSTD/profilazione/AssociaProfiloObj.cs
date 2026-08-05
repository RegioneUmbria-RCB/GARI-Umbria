using AgronicaCoreModelsSTD.utente;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public class AssociaProfiloObj
    {
        public List<UtentePermessi> Utenti { get; set; }
        public TipologiaUtente Profilo { get; set; }
        public bool AssociaImpostazioni { get; set; }
        /// <summary>
        /// Lista di impostazioni da propagare. Se vuota, considerare tutte le impostazioni del profilo.
        /// </summary>
        public IEnumerable<Utente_Impostazioni> Impostazioni { get; set; } = new List<Utente_Impostazioni>();
    }

    public  class CopyProfileObj
    {
        public TipologiaUtente Original {get; set;}
        public TipologiaUtente CopyTemplate { get; set; }
        public bool AlsoCopySettings { get; set; } = false;
    }
}
