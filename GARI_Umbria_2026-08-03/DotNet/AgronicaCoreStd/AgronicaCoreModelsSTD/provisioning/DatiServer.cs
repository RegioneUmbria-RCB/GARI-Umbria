using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.provisioning
{    
     /// <summary>
     /// Classe lista dati server
     /// </summary>
    public class DatiServer
    {    
         /// <summary>
         /// Lista dei dati dei server desiderati
         /// </summary>
        public List<ServerData> datiServer { get; set; }
    }

    /// <summary>
    /// Classe dati base server
    /// </summary>
    public class ServerData
    {
        /// <summary>
        /// Identificativo del server
        /// </summary>
        public int IDDb { get; set; }

        /// <summary>
        /// Descrizione testuale del server
        /// </summary>
        public String Descrizione { get; set; }
    }
}
