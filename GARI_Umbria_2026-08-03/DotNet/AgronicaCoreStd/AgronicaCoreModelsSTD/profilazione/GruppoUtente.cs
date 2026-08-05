using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    /// <summary>
    /// Classe che definisce un gruppo di appartenenza dell'utente (aka. profilo utente).
    /// 
    /// Il Gruppo Utente interagisce con i workflow aziendali.
    /// </summary>
    public class GruppoUtente: BaseCodeDescr
    {
        /// <summary>
        /// Identificativo del gruppo. Non è il codice univoco. Può essere modificato ed è visibile all'utente.
        /// </summary>
        public string Identificativo { get; set; }

        public GruppoUtente() : base() { }

        public GruppoUtente(int code, string descr) : base(code, descr) { }
    }

}
