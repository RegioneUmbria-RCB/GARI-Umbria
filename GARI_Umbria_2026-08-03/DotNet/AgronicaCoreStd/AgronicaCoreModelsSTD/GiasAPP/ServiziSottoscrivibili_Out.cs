using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.GiasAPP
{    
     /// <summary>
     /// Classe output dell'endpoint della lista dei servizi sottoscrivibili a notifica
     /// </summary>
    public class ServiziSottoscrivibili_Out
    {
        /// <summary>
        /// Lista dei servizi sottoscrivibili dall'utente
        /// </summary>
        public List<ServiziSottoscrivibili> ListaServizi { get; set; }
    }

    public class ServiziSottoscrivibili
    {
        /// <summary>
        /// Id Servizio
        /// </summary>
        public Int32 IdServizio { get; set; }
        /// <summary>
        /// Descrizione del servizio
        /// </summary>
        public String Descrizione { get; set; }
        /// <summary>
        /// flag di obbligatorietà della sottoscrizione
        /// </summary>
        public bool Obbligatorio { get; set; }
        /// <summary>
        /// flag Sottoscrivi
        /// </summary>
        public bool Sottoscrivi { get; set; }
        /// <summary>
        /// Parametro DeepLink della notifica
        /// </summary>
        public String DeepLink { get; set; }
        /// <summary>
        /// Parametro UrlMedia della notifica
        /// </summary>
        public String UrlMedia { get; set; }

    }
}
