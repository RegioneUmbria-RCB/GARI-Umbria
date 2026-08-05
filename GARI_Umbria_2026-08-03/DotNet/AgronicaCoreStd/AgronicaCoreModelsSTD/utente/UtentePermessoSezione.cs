using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.utente
{
    public class UtentePermessoSezione : Utente_Permesso
    {
        /// <summary>
        /// Modulo GIAS in cui è usato il permesso.
        /// </summary>
        public BaseCodeDescr Modulo { get; set; } = new BaseCodeDescr();
        /// <summary>
        /// Pagina in cui è usato il permesso.
        /// </summary>
        public BaseCodeDescr Pagina { get; set; } = new BaseCodeDescr();
        /// <summary>
        /// Descrizione della funzione a cui fa riferimento il permesso.
        /// </summary>
        public string Funzione { get; set; }
    }
}
