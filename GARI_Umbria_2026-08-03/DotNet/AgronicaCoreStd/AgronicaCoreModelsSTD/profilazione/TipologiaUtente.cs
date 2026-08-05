using AgronicaCoreModelsSTD.utente;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    /// <summary>
    /// Classe che definisce una tipologia utente (aka. profilo utente).
    ///
    /// La tipologia utente racchiude indicazioni sui permessi posseduti dall'utente.
    /// </summary>
    public class TipologiaUtente : baseClass.BaseCodeDescr
    {
        public string Note { get; set; } = "";
        public List<Utente_Permesso> Permessi;

        public TipologiaUtente() { }

        public TipologiaUtente(int cod, string descr): base(cod, descr)
        {

        }
    }
}
